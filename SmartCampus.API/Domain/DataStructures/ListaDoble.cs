namespace SmartCampus.API.Domain.DataStructures
{
    // El nodo doble tiene referencia al anterior Y al siguiente
    // Esto permite navegar en ambas direcciones
    public class NodoDoble<T>
    {
        public T Dato { get; set; }
        public NodoDoble<T>? Siguiente { get; set; }
        public NodoDoble<T>? Anterior { get; set; }

        public NodoDoble(T dato)
        {
            Dato = dato;
            Siguiente = null;
            Anterior = null;
        }
    }

    public class ListaDoble<T>
    {
        private NodoDoble<T>? _cabeza;   // primer nodo
        private NodoDoble<T>? _cola;     // último nodo
        private NodoDoble<T>? _actual;   // posición actual del cursor
        private int _tamanio;

        public ListaDoble()
        {
            _cabeza = null;
            _cola = null;
            _actual = null;
            _tamanio = 0;
        }

        // Insertar al final
        // Uso real: agregar un registro al expediente del estudiante
        public void InsertarAlFinal(T dato)
        {
            var nuevoNodo = new NodoDoble<T>(dato);

            if (_cabeza == null)
            {
                _cabeza = nuevoNodo;
                _cola = nuevoNodo;
                _actual = nuevoNodo;
            }
            else
            {
                nuevoNodo.Anterior = _cola;
                _cola!.Siguiente = nuevoNodo;
                _cola = nuevoNodo;
            }
            _tamanio++;
        }

        // Mover al siguiente registro
        // Uso real: botón "Siguiente" en la vista del expediente
        public T? Siguiente()
        {
            if (_actual?.Siguiente == null)
                return default;

            _actual = _actual.Siguiente;
            return _actual.Dato;
        }

        // Mover al registro anterior
        // Uso real: botón "Anterior" en la vista del expediente
        public T? Anterior()
        {
            if (_actual?.Anterior == null)
                return default;

            _actual = _actual.Anterior;
            return _actual.Dato;
        }

        // Obtener el dato en la posición actual
        public T? ObtenerActual()
        {
            return _actual == null ? default : _actual.Dato;
        }

        // Ir al primer registro
        public T? IrAlPrimero()
        {
            _actual = _cabeza;
            return ObtenerActual();
        }

        // Ir al último registro
        public T? IrAlUltimo()
        {
            _actual = _cola;
            return ObtenerActual();
        }

        // Saber si hay un siguiente disponible
        public bool TieneSiguiente() => _actual?.Siguiente != null;

        // Saber si hay un anterior disponible
        public bool TieneAnterior() => _actual?.Anterior != null;

        // Obtener todos los elementos
        public List<T> ObtenerTodos()
        {
            var resultado = new List<T>();
            var nodo = _cabeza;
            while (nodo != null)
            {
                resultado.Add(nodo.Dato);
                nodo = nodo.Siguiente;
            }
            return resultado;
        }

        public bool EstaVacia() => _cabeza == null;
        public int Tamanio() => _tamanio;
    }
}