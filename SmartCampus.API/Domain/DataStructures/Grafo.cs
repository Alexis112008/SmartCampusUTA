namespace SmartCampus.API.Domain.DataStructures
{
    // Cada nodo representa un edificio del campus
    public class NodoGrafo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double PosX { get; set; }
        public double PosY { get; set; }

        public NodoGrafo(int id, string nombre, double posX, double posY)
        {
            Id = id;
            Nombre = nombre;
            PosX = posX;
            PosY = posY;
        }
    }

    // Cada arista representa un camino entre dos edificios
    public class Arista
    {
        public int IdDestino { get; set; }
        public double Distancia { get; set; }

        public Arista(int idDestino, double distancia)
        {
            IdDestino = idDestino;
            Distancia = distancia;
        }
    }

    public class Grafo
    {
        // Lista de adyacencia: cada nodo tiene su lista de vecinos
        private Dictionary<int, List<Arista>> _adyacencia;
        private Dictionary<int, NodoGrafo> _nodos;

        public Grafo()
        {
            _adyacencia = new Dictionary<int, List<Arista>>();
            _nodos = new Dictionary<int, NodoGrafo>();
        }

        // Agregar un edificio (nodo) al grafo
        public void AgregarNodo(int id, string nombre,
                                 double posX, double posY)
        {
            if (!_nodos.ContainsKey(id))
            {
                _nodos[id] = new NodoGrafo(id, nombre, posX, posY);
                _adyacencia[id] = new List<Arista>();
            }
        }

        // Agregar un camino (arista) entre dos edificios
        // Uso real: conectar dos edificios con su distancia en metros
        public void AgregarArista(int idOrigen, int idDestino,
                                   double distancia)
        {
            if (!_adyacencia.ContainsKey(idOrigen)) return;
            _adyacencia[idOrigen].Add(new Arista(idDestino, distancia));
        }

        // BFS: Búsqueda en anchura para encontrar la ruta más corta
        // (en grafos sin pesos, BFS garantiza el menor número de saltos)
        // Uso real: "¿Cómo llego de Rectorado a la Cafetería?"
        public List<string> BuscarRuta(int idOrigen, int idDestino)
        {
            if (!_nodos.ContainsKey(idOrigen) ||
                !_nodos.ContainsKey(idDestino))
                return new List<string> { "Nodo no encontrado" };

            // Visitados: evita recorrer el mismo nodo dos veces
            var visitados = new HashSet<int>();

            // Cola BFS: guardamos el camino recorrido hasta cada nodo
            var colaBFS = new Queue<List<int>>();
            colaBFS.Enqueue(new List<int> { idOrigen });
            visitados.Add(idOrigen);

            while (colaBFS.Count > 0)
            {
                var caminoActual = colaBFS.Dequeue();
                var nodoActual = caminoActual[^1]; // último elemento

                // Llegamos al destino
                if (nodoActual == idDestino)
                {
                    // Convertir IDs a nombres de edificios
                    return caminoActual
                        .Select(id => _nodos[id].Nombre)
                        .ToList();
                }

                // Explorar vecinos
                foreach (var arista in _adyacencia[nodoActual])
                {
                    if (!visitados.Contains(arista.IdDestino))
                    {
                        visitados.Add(arista.IdDestino);
                        var nuevoCamino = new List<int>(caminoActual)
                        {
                            arista.IdDestino
                        };
                        colaBFS.Enqueue(nuevoCamino);
                    }
                }
            }

            return new List<string> { "No existe ruta entre los puntos" };
        }

        // Obtener todos los nodos (para dibujar el mapa)
        public List<NodoGrafo> ObtenerNodos() =>
            _nodos.Values.ToList();

        // Obtener vecinos de un nodo
        public List<Arista> ObtenerVecinos(int idNodo)
        {
            return _adyacencia.ContainsKey(idNodo)
                ? _adyacencia[idNodo]
                : new List<Arista>();
        }

        public bool EstaVacio() => _nodos.Count == 0;
        public int TotalNodos() => _nodos.Count;
    }
}