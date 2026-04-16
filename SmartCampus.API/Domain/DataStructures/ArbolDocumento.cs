namespace SmartCampus.API.Domain.DataStructures
{
    // Cada nodo del árbol representa una categoría o documento
    public class NodoArbol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        // Tipo: "Categoria" o "Documento"
        public int Nivel { get; set; }

        // Un nodo puede tener muchos hijos
        public List<NodoArbol> Hijos { get; set; } = new List<NodoArbol>();

        public NodoArbol(int id, string nombre, string tipo, int nivel)
        {
            Id = id;
            Nombre = nombre;
            Tipo = tipo;
            Nivel = nivel;
        }
    }

    public class ArbolDocumento
    {
        private NodoArbol? _raiz;

        public ArbolDocumento()
        {
            _raiz = null;
        }

        // Insertar nodo raíz (primera categoría principal)
        public void InsertarRaiz(int id, string nombre)
        {
            _raiz = new NodoArbol(id, nombre, "Categoria", 0);
        }

        // Insertar un hijo en un nodo padre específico
        // Uso real: agregar subcategoría o documento dentro de una categoría
        public bool InsertarHijo(int idPadre, int idNuevo,
                                  string nombre, string tipo)
        {
            if (_raiz == null) return false;
            var padre = BuscarNodo(_raiz, idPadre);

            if (padre == null) return false;

            var nuevoNodo = new NodoArbol(
                idNuevo, nombre, tipo, padre.Nivel + 1);
            padre.Hijos.Add(nuevoNodo);
            return true;
        }

        // Buscar un nodo por su ID (recorrido DFS recursivo)
        private NodoArbol? BuscarNodo(NodoArbol nodo, int id)
        {
            if (nodo.Id == id) return nodo;

            foreach (var hijo in nodo.Hijos)
            {
                var encontrado = BuscarNodo(hijo, id);
                if (encontrado != null) return encontrado;
            }
            return null;
        }

        // Obtener el árbol completo desde la raíz
        // Uso real: mostrar el árbol de documentos en pantalla
        public NodoArbol? ObtenerArbol() => _raiz;

        // Obtener hijos directos de un nodo
        // Uso real: expandir una categoría al hacer clic
        public List<NodoArbol> ObtenerHijos(int idPadre)
        {
            if (_raiz == null) return new List<NodoArbol>();
            var padre = BuscarNodo(_raiz, idPadre);
            return padre?.Hijos ?? new List<NodoArbol>();
        }

        // Recorrido en preorden: raíz → hijos de izq a der
        // Uso real: listar todos los documentos en orden jerárquico
        public List<NodoArbol> RecorridoPreorden()
        {
            var resultado = new List<NodoArbol>();
            if (_raiz != null)
                PreordenRecursivo(_raiz, resultado);
            return resultado;
        }

        private void PreordenRecursivo(NodoArbol nodo,
                                        List<NodoArbol> resultado)
        {
            resultado.Add(nodo);
            foreach (var hijo in nodo.Hijos)
                PreordenRecursivo(hijo, resultado);
        }

        public bool EstaVacio() => _raiz == null;
    }
}