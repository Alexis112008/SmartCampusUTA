namespace SmartCampus.API.Domain.DataStructures
{
    // La pila funciona LIFO: Last In First Out
    // El último en entrar es el primero en salir
    // Igual que una pila de platos: sacas el de arriba primero
    public class NodoPila<T>
    {
        public T Dato { get; set; }
        public NodoPila<T>? Siguiente { get; set; }

        public NodoPila(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    public class Pila<T>
    {
        private NodoPila<T>? _tope;   // el nodo más reciente
        private int _tamanio;

        public Pila()
        {
            _tope = null;
            _tamanio = 0;
        }

        // Push: agregar al tope de la pila
        // Uso real: registrar una acción que se puede deshacer
        public void Push(T dato)
        {
            var nuevoNodo = new NodoPila<T>(dato);
            nuevoNodo.Siguiente = _tope;
            _tope = nuevoNodo;
            _tamanio++;
        }

        // Pop: sacar del tope (elimina y devuelve)
        // Uso real: deshacer la última acción del usuario
        public T Pop()
        {
            if (_tope == null)
                throw new InvalidOperationException(
                    "No hay acciones para deshacer.");

            var dato = _tope.Dato;
            _tope = _tope.Siguiente;
            _tamanio--;
            return dato;
        }

        // Peek: ver el tope sin sacarlo
        // Uso real: mostrar "¿Deseas deshacer: [última acción]?"
        public T Peek()
        {
            if (_tope == null)
                throw new InvalidOperationException(
                    "La pila está vacía.");

            return _tope.Dato;
        }

        public bool EstaVacia() => _tope == null;
        public int Tamanio() => _tamanio;

        // Ver todo el historial de acciones pendientes
        public List<T> ObtenerTodos()
        {
            var resultado = new List<T>();
            var actual = _tope;
            while (actual != null)
            {
                resultado.Add(actual.Dato);
                actual = actual.Siguiente;
            }
            return resultado;
        }
    }
}