using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using VDS.RDF;

namespace Robo
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new Graph();
            var path = "C:\\GitRepositories\\ROBO\\RoboOntology.owl";

            graph.LoadFromFile(path);
            graph.LoadFromUri(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns"));
            graph.LoadFromUri(new Uri("http://www.w3.org/2000/01/rdf-schema"));

            var kings = GetKingsFromCsv();

            foreach (var king in kings)
            {
                var kingUri = new Uri(king.Uri);
                var node = graph.CreateUriNode(kingUri);
                var kingNode = graph.GetUriNode(new Uri(RoboEnum.GetRoboUri("King")));
                var cityNode = graph.GetUriNode(new Uri(RoboEnum.GetRoboUri("City")));
                var typeNode = graph.GetUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
                var ruledFromNode = graph.GetUriNode(new Uri(RoboEnum.GetRoboUri("RuledFrom")));
                var ruledToNode = graph.GetUriNode(new Uri(RoboEnum.GetRoboUri("RuledTo")));
                var babylon = graph.GetUriNode(new Uri(RoboEnum.GetRoboUri("Babylon")));
                var successor = graph.GetUriNode(new Uri(RoboEnum.GetRoboUri("Successor")));

                var triple = new Triple(node, typeNode, kingNode);
                var ruledFromTriple = new Triple(node, ruledFromNode, graph.CreateLiteralNode(king.RuledFrom.ToString()));
                var ruledToTriple = new Triple(node, ruledToNode, graph.CreateLiteralNode(king.RuledTo.ToString()));
                var cityTriple = new Triple(node, cityNode, babylon);

                if (!string.IsNullOrEmpty(king.SuccessorUri))
                {
                    var ruledSuccessorTriple = new Triple(node, successor, graph.CreateUriNode(new Uri(king.SuccessorUri)));
                    graph.Assert(ruledSuccessorTriple);
                }

                graph.Assert(triple);
                graph.Assert(ruledFromTriple);
                graph.Assert(ruledToTriple);
                graph.Assert(cityTriple);

                Console.WriteLine(king.Uri + " - " + king.SuccessorUri);
            }
            
            graph.SaveToFile(path);
        }

        private static List<RoboKing> GetKingsFromCsv()
        {
            var engine = new FileHelperEngine<King>();

            var csvKings = engine.ReadFile("C:\\GitRepositories\\ROBO\\BabylonianKings.csv");

            engine.WriteFile("FileOut.txt", csvKings);

            var kings = new List<RoboKing>();

            var i = 1;

            foreach (var king in csvKings.Skip(1))
            {
                i = i + 1;

                var dates = king.Reigned.Split('–');
                var ruledFrom = 0;
                var ruledTo = 0;

                switch (dates.Length)
                {
                    case 0:
                    case 1 when !int.TryParse(dates[0], out ruledFrom):
                    {
                        break;
                    }
                }

                if (dates.Length > 1)
                {
                    int.TryParse(dates[0], out ruledFrom);

                    int.TryParse(dates[1], out ruledTo);
                }

                var roboKing = new RoboKing
                {
                    RuledFrom = ruledFrom,
                    RuledTo = ruledTo,
                    Uri = RoboEnum.GetRoboUri(king.Ruler)
                };
                
                if (i < csvKings.Length)
                {
                   var csvKing = csvKings[i];

                   roboKing.SuccessorUri = RoboEnum.GetRoboUri(csvKing.Ruler);
                }

                kings.Add(roboKing);
            }

            return kings;
        }
    }
}
