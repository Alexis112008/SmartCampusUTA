namespace SmartCampus.API.Domain.DataStructures
{
    // El último nodo apunta de vuelta al primero
    // Esto crea un ciclo continuo sin fin
    public class NodoCircular<T>
    {
        public T Dato { get; set; }
        public NodoCircular<T>? Siguiente { get; set; }

        public NodoCircular(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    public class ListaCircular<T>
    {
        private NodoCircular<T>? _ultimo;  // apuntamos al último
        private int _tamanio;              // porque desde él llegamos al primero

        public ListaCircular()
        {
            _ultimo = null;
            _tamanio = 0;
        }

        // Insertar nuevo elemento al final del ciclo
        // Uso real: registrar una ventanilla activa
        public void Insertar(T dato)
        {
            var nuevoNodo = new NodoCircular<T>(dato);

            if (_ultimo == null)
            {
                // Único elemento: se apunta a sí mismo
                nuevoNodo.Siguiente = nuevoNodo;
                _ultimo = nuevoNodo;
            }
            else
            {
                // El nuevo nodo apunta al que era el primero
                nuevoNodo.Siguiente = _ultimo.Siguiente;
                // El último apunta al nuevo
                _ultimo.Siguiente = nuevoNodo;
                // El nuevo es ahora el último
                _ultimo = nuevoNodo;
            }
            _tamanio++;
        }

        // Obtener el siguiente en la rotación y avanzar
        // Uso real: rotar automáticamente entre ventanillas
        public T? ObtenerSiguienteEnRotacion()
        {
            if (_ultimo == null) return default;

            // El "primero" es el que sigue al último
            var primero = _ultimo.Siguiente;
            // Avanzamos el puntero (rotamos)
            _ultimo = primero;
            return primero!.Dato;
        }

        // Ver el actual sin avanzar
        public T? VerActual()
        {
            if (_ultimo == null) return default;
            return _ultimo.Siguiente!.Dato;
        }

        // Obtener todos los elementos una vuelta completa
        public List<T> ObtenerTodos()
        {
            var resultado = new List<T>();
            if (_ultimo == null) return resultado;

            var inicio = _ultimo.Siguiente;
            var actual = inicio;
            do
            {
                resultado.Add(actual!.Dato);
                actual = actual.Siguiente;
            } while (actual != inicio);

            return resultado;
        }

        public bool EstaVacia() => _ultimo == null;
        public int Tamanio() => _tamanio;
    }
}