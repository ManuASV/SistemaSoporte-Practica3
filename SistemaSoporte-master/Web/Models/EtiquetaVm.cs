namespace Web.Models;

/// <summary>
/// Forma que ESTE proyecto necesita de una etiqueta del API (JSON: id, nombre, color).
/// Es una clase propia del MVC: no se comparte con el API, el contrato entre los dos es el JSON.
/// </summary>
public class EtiquetaVm
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Color { get; set; } = "#6c757d";
}
