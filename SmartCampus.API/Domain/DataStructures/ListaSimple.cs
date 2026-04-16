namespace SmartCampus.API.Domain.DataStructures
{
    // El nodo es la unidad básica de la lista
    // Contiene el dato y la referencia al siguiente nodo
    public class NodoSimple<T>
    {
        public T Dato { get; set; }
        public NodoSimple<T>? Siguiente { get; set; }

        public NodoSimple(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    // La lista encadena nodos uno tras otro
    // Solo se puede recorrer hacia adelante
    public class ListaSimple<T>
    {
        private NodoSimple<T>? _cabeza;  // primer nodo
        private int _tamanio;

        public ListaSimple()
        {
            _cabeza = null;
            _tamanio = 0;
        }

        // Insertar al final de la lista
        // Uso real: agregar un nuevo estado al historial del trámite
        public void InsertarAlFinal(T dato)
        {
            var nuevoNodo = new NodoSimple<T>(dato);

            if (_cabeza == null)
            {
                // Lista vacía: el nuevo nodo es la cabeza
                _cabeza = nuevoNodo;
            }
            else
            {
                // Recorrer hasta el último nodo
                var actual = _cabeza;
                while (actual.Siguiente != null)
                    actual = actual.Siguiente;

                // El último nodo apunta al nuevo
                actual.Siguiente = nuevoNodo;
            }
            _tamanio++;
        }

        // Insertar al inicio
        // Uso real: mostrar el estado más reciente primero
        public void InsertarAlInicio(T dato)
        {
            var nuevoNodo = new NodoSimple<T>(dato);
            nuevoNodo.Siguiente = _cabeza;
            _cabeza = nuevoNodo;
            _tamanio++;
        }

        // Recorrer y devolver todos los elementos
        // Uso real: mostrar el historial completo de un trámite
        public List<T> ObtenerTodos()
        {
            var resultado = new List<T>();
            var actual = _cabeza;

            while (actual != null)
            {
                resultado.Add(actual.Dato);
                actual = actual.Siguiente;
            }
            return resultado;
        }

        // Verificar si la lista está vacía
        public bool EstaVacia() => _cabeza == null;

        // Tamaño actual
        public int Tamanio() => _tamanio;

        // Eliminar el primer nodo
        public void EliminarPrimero()
        {
            if (_cabeza == null) return;
            _cabeza = _cabeza.Siguiente;
            _tamanio--;
        }
    }
}