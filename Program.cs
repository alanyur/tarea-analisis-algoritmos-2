using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TareaAnalisisAlgoritmos
{
    class Program
    {
        const long INF = long.MaxValue / 2; // Previene overflow en sumas

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.WriteLine("Iniciando programa de la Tarea 2...\n");

            // Parte 2.1: Verificar correctitud
            VerifyCorrectness();
            
            // Parte 2.2: Experimentos y tiempos
            Console.WriteLine("Iniciando experimentos (Parte 2.2)...");
            RunExperiments();
            
            // Parte 2.3: Dataset real
            Console.WriteLine("\nProcesando dataset Chebyshev2 (Parte 2.3)...");
            ProcessChebyshevDataset("chebyshev2.mtx"); // Asegúrate de tener el archivo en el directorio
            
            Console.WriteLine("\nEjecución finalizada. Presiona cualquier tecla para salir.");
            Console.ReadKey();
        }

        // --- PARTE 2.1: Verificación de Correctitud ---
        static void VerifyCorrectness()
        {
            Console.WriteLine("--- Verificación de Correctitud (Parte 2.1) ---");
            int V = 4;
            
            // Grafo de prueba dirigido:
            // 0 -> 1 (peso 5)
            // 0 -> 3 (peso 10)
            // 1 -> 2 (peso 3)
            // 2 -> 3 (peso 1)
            // El camino más corto de 0 a 3 debería ser 0->1->2->3 (peso 9)
            
            List<(int u, int v, long w)> edges = new List<(int, int, long)>
            {
                (0, 1, 5), (0, 3, 10),
                (1, 2, 3),
                (2, 3, 1)
            };

            // Construir matriz de adyacencia para FW
            long[,] matrix = new long[V, V];
            for (int i = 0; i < V; i++)
                for (int j = 0; j < V; j++)
                    matrix[i, j] = (i == j) ? 0 : INF;

            foreach (var edge in edges)
                matrix[edge.u, edge.v] = edge.w;

            Console.WriteLine("Resultado Algoritmo Base (Bellman-Ford):");
            var distBase = RunBaseAlgorithm(V, edges);
            PrintMatrix(distBase, V);

            Console.WriteLine("\nResultado Algoritmo de Floyd-Warshall:");
            var distFW = RunFloydWarshall(V, matrix);
            PrintMatrix(distFW, V);
            Console.WriteLine("-----------------------------------------------\n");
        }

        static void PrintMatrix(long[,] dist, int V)
        {
            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < V; j++)
                {
                    if (dist[i, j] == INF) 
                        Console.Write("INF\t");
                    else 
                        Console.Write(dist[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

        // --- ALGORITMOS ---

        // Algoritmo Base (n llamadas a Bellman-Ford)
        static long[,] RunBaseAlgorithm(int V, List<(int u, int v, long w)> edges)
        {
            long[,] allDistances = new long[V, V];
            
            for (int s = 0; s < V; s++)
            {
                long[] dist = new long[V];
                Array.Fill(dist, INF);
                dist[s] = 0;

                for (int i = 0; i < V - 1; i++)
                {
                    bool relaxed = false;
                    foreach (var edge in edges)
                    {
                        if (dist[edge.u] != INF && dist[edge.u] + edge.w < dist[edge.v])
                        {
                            dist[edge.v] = dist[edge.u] + edge.w;
                            relaxed = true;
                        }
                    }
                    if (!relaxed) break; // Optimización si no hay cambios
                }
                
                for (int j = 0; j < V; j++)
                    allDistances[s, j] = dist[j];
            }
            return allDistances;
        }

        // Algoritmo de Floyd-Warshall
        static long[,] RunFloydWarshall(int V, long[,] adjMatrix)
        {
            long[,] dist = new long[V, V];
            Array.Copy(adjMatrix, dist, adjMatrix.Length);

            for (int k = 0; k < V; k++)
            {
                for (int i = 0; i < V; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        if (dist[i, k] != INF && dist[k, j] != INF && dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                        }
                    }
                }
            }
            return dist;
        }

        // --- EXPERIMENTOS (Parte 2.2) ---
        
        static void RunExperiments()
        {
            int[] sizes = { 10, 25, 50, 75, 100 }; // Tamaños de N (Vértices)
            int runs = 32; // Mínimo 32 ejecuciones según requerimiento
            
            using (StreamWriter writer = new StreamWriter("experimentos_resultados.csv"))
            {
                writer.WriteLine("ExperimentType,N,Algorithm,MeanTimeMs,StdDevMs");

                string[] experimentTypes = { "Densidad_10", "Densidad_90", "Completo", "Cadena" };

                foreach (string expType in experimentTypes)
                {
                    foreach (int N in sizes)
                    {
                        var (edges, matrix) = GenerateGraph(N, expType);

                        // Medir Algoritmo Base
                        var baseStats = MeasureTime(() => RunBaseAlgorithm(N, edges), runs);
                        writer.WriteLine($"{expType},{N},Base,{baseStats.mean},{baseStats.stdDev}");

                        // Medir Floyd-Warshall
                        var fwStats = MeasureTime(() => RunFloydWarshall(N, matrix), runs);
                        writer.WriteLine($"{expType},{N},FloydWarshall,{fwStats.mean},{fwStats.stdDev}");
                        
                        Console.WriteLine($"Completado: {expType} N={N}");
                    }
                }
            }
            Console.WriteLine("Resultados de experimentos exportados a experimentos_resultados.csv");
        }

        static (double mean, double stdDev) MeasureTime(Action algorithm, int runs)
        {
            double[] times = new double[runs];
            Stopwatch sw = new Stopwatch();

            // Calentamiento (evita que el compilador JIT afecte la primera medición)
            algorithm();

            for (int i = 0; i < runs; i++)
            {
                sw.Restart();
                algorithm();
                sw.Stop();
                times[i] = sw.Elapsed.TotalMilliseconds;
            }

            double mean = times.Average();
            double sumSquares = times.Sum(t => Math.Pow(t - mean, 2));
            double stdDev = Math.Sqrt(sumSquares / runs);

            return (mean, stdDev);
        }

        // Generador de instancias
        static (List<(int u, int v, long w)> edges, long[,] matrix) GenerateGraph(int V, string type)
        {
            List<(int u, int v, long w)> edges = new List<(int, int, long)>();
            long[,] matrix = new long[V, V];
            Random rnd = new Random(42); // Semilla fija para reproducibilidad

            for (int i = 0; i < V; i++)
                for (int j = 0; j < V; j++)
                    matrix[i, j] = (i == j) ? 0 : INF;

            double probability = type == "Densidad_10" ? 0.1 : (type == "Densidad_90" ? 0.9 : 1.0);

            if (type == "Cadena")
            {
                for (int i = 0; i < V - 1; i++)
                {
                    long w = rnd.Next(1, 100);
                    edges.Add((i, i + 1, w));
                    matrix[i, i + 1] = w;
                }
            }
            else
            {
                for (int i = 0; i < V; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        if (i != j && rnd.NextDouble() <= probability)
                        {
                            long w = rnd.Next(1, 100);
                            edges.Add((i, j, w));
                            matrix[i, j] = w;
                        }
                    }
                }
            }
            return (edges, matrix);
        }

        // --- DATASET REAL (Parte 2.3) ---

        static void ProcessChebyshevDataset(string filepath)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine($"Archivo {filepath} no encontrado. Descárgalo de NetworkRepository y colócalo en la misma carpeta que el ejecutable.");
                return;
            }

            // Lectura básica de archivo MTX (asume nodos 1-indexados)
            var lines = File.ReadAllLines(filepath).Where(l => !l.StartsWith("%")).ToList();
            if (lines.Count == 0) return;

            string[] header = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int V = int.Parse(header[0]); // Cantidad de Nodos
            
            long[,] matrix = new long[V, V];
            for (int i = 0; i < V; i++)
                for (int j = 0; j < V; j++)
                    matrix[i, j] = (i == j) ? 0 : INF;

            Random rnd = new Random(42);

            for (int i = 1; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    int u = int.Parse(parts[0]) - 1;
                    int v = int.Parse(parts[1]) - 1;
                    // Genera peso aleatorio del 1 al 10 si el dataset no tiene la tercera columna de pesos
                    long w = parts.Length > 2 ? (long)double.Parse(parts[2]) : rnd.Next(1, 10); 
                    matrix[u, v] = w;
                }
            }

            Stopwatch sw = Stopwatch.StartNew();
            long[,] distances = RunFloydWarshall(V, matrix);
            sw.Stop();
            Console.WriteLine($"Grafo Chebyshev2 procesado por Floyd-Warshall en {sw.ElapsedMilliseconds} ms.");

            // Exportar a CSV
            using (StreamWriter writer = new StreamWriter("chebyshev_resultados.csv"))
            {
                writer.WriteLine("Nodo1,Nodo2,Distancia");
                for (int i = 0; i < V; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        if (distances[i, j] != INF && i != j)
                        {
                            writer.WriteLine($"{i + 1},{j + 1},{distances[i, j]}");
                        }
                    }
                }
            }
            Console.WriteLine("Resultados del dataset exportados a chebyshev_resultados.csv");
        }
    }
}