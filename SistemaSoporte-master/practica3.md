1. Post-Redirect-Get

Es un patrón donde el servidor, después de procesar un POST, no devuelve una página directamente sino una redirección a un GET. En EtiquetasController.Crear, tras crear la etiqueta se hace RedirectToAction(nameof(Detalle), ...). Si se omitiera y se devolviera View(datos), la página que el navegador muestra sería la respuesta a un POST. Al presionar F5 el navegador pide confirmar el reenvío del formulario y, si el usuario acepta, repite el POST. Eso crearía la etiqueta otra vez o daría un 409 por nombre repetido. Con PRG, F5 solo repite el GET del detalle, que no modifica nada, y la URL final se puede guardar o compartir sin problema.

Fuente: Wikipedia, "Post/Redirect/Get". https://en.wikipedia.org/wiki/Post/Redirect/Get

2. Tiempo de vida del cliente tipado

Un cliente tipado registrado con AddHttpClient<IEtiquetasApi, EtiquetasApi> es Transient: cada vez que se inyecta, se crea una instancia nueva de EtiquetasApi con un HttpClient configurado por la fábrica (URL base, timeout y TokenApiHandler). La fábrica administra aparte las conexiones (los handlers) y las recicla cada cierto tiempo. Si fuera Singleton, el cliente viviría para siempre con un mismo handler y no se enteraría de cambios de DNS. Si se hiciera new HttpClient() en cada petición, se abrirían y cerrarían conexiones sin control y se pueden agotar los sockets. Además se saltaría la configuración central y el token no viajaría.

Fuente: Microsoft Learn, "Make HTTP requests using IHttpClientFactory in ASP.NET Core". https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests y "IHttpClientFactory with .NET". https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory

3. ProblemDetails (RFC 9457)

Es un formato estándar en JSON (application/problem+json) para que un API describa sus errores de forma uniforme. Sus campos son type, title, status, detail e instance. Mi ApiException lee title, detail y errors, y toma el código del estado HTTP de la respuesta. Un 400 de validación trae además errors, un diccionario con los mensajes por campo (por ejemplo Nombre y Color), y por eso el MVC los muestra bajo cada campo. Un 409 por nombre repetido trae title y detail, pero no errors, y por eso el controlador lo muestra como mensaje general del formulario.

Fuente: IETF, RFC 9457, "Problem Details for HTTP APIs". https://www.rfc-editor.org/rfc/rfc9457

4. ValidateAntiForgeryToken

Previene el ataque CSRF (falsificación de petición entre sitios): una página maliciosa hace que el navegador de un usuario ya autenticado envíe una petición a nuestro sitio, y el navegador adjunta la cookie de sesión automáticamente. ASP.NET Core lo evita con dos tokens: uno va en una cookie y otro en un campo oculto del formulario (__RequestVerificationToken). [ValidateAntiForgeryToken] comprueba al recibir el POST que ambos coincidan, y un sitio ajeno no puede leer ni generar ese par. En el formulario, <form method="post" asp-action="Crear"> agrega el campo oculto automáticamente.

Fuente: Microsoft Learn, "Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core". https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery

5. ViewData, ViewBag, TempData y modelo tipado

El modelo tipado lleva los datos principales de la vista, con tipo y validación; en mi práctica, EtiquetaCrearVm en el formulario y EtiquetaVm en el detalle. ViewData es un diccionario sin tipo del controlador a la vista y solo dura esa petición; lo usaría para banderas pequeñas, como ViewData["MostrarCliente"] en Tickets/Index. ViewBag es lo mismo, pero con sintaxis dynamic, y pierde la revisión del compilador. TempData sobrevive a una redirección y se lee una sola vez; por eso TempData["Exito"] muestra el mensaje verde tras crear una etiqueta y redirigir al detalle.

Fuente: Microsoft Learn, "Views in ASP.NET Core MVC". https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview y "Session and state management in ASP.NET Core". https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state