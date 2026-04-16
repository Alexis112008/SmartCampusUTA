namespace SmartCampus.API.Domain.DataStructures
{
    // La cola funciona FIFO: First In First Out
    // El primero en entrar es el primero en salir
    // Igual que una fila del banco: el primero que llegó, primero es atendido
    public class NodoCola<T>
    {
        public T Dato { get; set; }
        public NodoCola<T>? Siguiente { get; set; }

        public NodoCola(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    public class Cola<T>
    {
        private NodoCola<T>? _frente;  // primer nodo (próximo a salir)
        private NodoCola<T>? _final;   // último nodo (último en llegar)
        private int _tamanio;

        public Cola()
        {
            _frente = null;
            _final = null;
            _tamanio = 0;
        }

        // Encolar: agregar al final de la fila
        // Uso real: estudiante solicita un turno
        public void Encolar(T dato)
        {
            var nuevoNodo = new NodoCola<T>(dato);

            if (_final == null)
            {
                // Cola vacía: el nodo es frente y final
                _frente = nuevoNodo;
                _final = nuevoNodo;
            }
            else
            {
                // El último nodo apunta al nuevo
                _final.Siguiente = nuevoNodo;
                _final = nuevoNodo;
            }
            _tamanio++;
        }

        // Desencolar: sacar del frente de la fila
        // Uso real: secretaria llama al siguiente turno
        public T Desencolar()
        {
            if (_frente == null)
                throw new InvalidOperationException(
                    "No hay turnos en espera.");

            var dato = _frente.Dato;
            _frente = _frente.Siguiente;

            // Si la cola quedó vacía, el final también es null
            if (_frente == null)
                _final = null;

            _tamanio--;
            return dato;
        }

        // Ver quién es el siguiente sin sacarlo
        // Uso real: mostrar "Próximo turno: T-005"
        public T VerFrente()
        {
            if (_frente == null)
                throw new InvalidOperationException(
                    "No hay turnos en espera.");

            return _frente.Dato;
        }

        public bool EstaVacia() => _frente == null;
        public int Tamanio() => _tamanio;

        // Ver todos los turnos en espera
        public List<T> ObtenerTodos()
        {
            var resultado = new List<T>();
            var actual = _frente;
            while (actual != null)
            {
                resultado.Add(actual.Dato);
                actual = actual.Siguiente;
            }
            return resultado;
        }
    }
}