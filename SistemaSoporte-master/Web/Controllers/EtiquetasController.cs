using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Servicios;

namespace Web.Controllers;

/// <summary>
/// Lista, detalle y creación de etiquetas. Todo el HTTP vive en IEtiquetasApi;
/// los errores que no se manejan aquí (API apagado, 401, 403, 500) los atrapa ApiExceptionFilter.
/// La protección por inicio de sesión es la FallbackPolicy de Program.cs: no se agrega nada aquí.
/// </summary>
public class EtiquetasController : Controller
{
    private readonly IEtiquetasApi _api;

    public EtiquetasController(IEtiquetasApi api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _api.ListarAsync(ct));

    public async Task<IActionResult> Detalle(int id, CancellationToken ct)
    {
        var etiqueta = await _api.ObtenerAsync(id, ct);
        return etiqueta is null ? NotFound() : View(etiqueta);
    }

    [HttpGet]
    public IActionResult Crear() => View(new EtiquetaCrearVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(EtiquetaCrearVm datos, CancellationToken ct)
    {
        // 1) El MVC valida las anotaciones del ViewModel.
        if (!ModelState.IsValid) return View(datos);

        try
        {
            var creada = await _api.CrearAsync(datos, ct);
            TempData["Exito"] = $"Etiqueta '{creada.Nombre}' creada.";

            // Post-Redirect-Get: si aquí se devolviera View(...), F5 reenviaría el POST y duplicaría la creación.
            return RedirectToAction(nameof(Detalle), new { id = creada.Id });
        }
        // 2) El API rechaza: 400 (errores por campo) o 409 (nombre repetido).
        catch (ApiException ex) when (ex.Codigo is 400 or 409)
        {
            AgregarErroresDelApi(ex);
            return View(datos);
        }
    }

    private void AgregarErroresDelApi(ApiException ex)
    {
        // 409: sin errores por campo -> mensaje general. 400: un mensaje por campo (Nombre, Color).
        if (ex.Errores.Count == 0) ModelState.AddModelError(string.Empty, ex.Message);
        foreach (var (campo, mensajes) in ex.Errores)
            foreach (var mensaje in mensajes)
                ModelState.AddModelError(campo, mensaje);
    }
}
