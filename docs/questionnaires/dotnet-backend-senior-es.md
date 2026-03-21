# Desarrollador Senior Backend .NET

## Q1
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre `ValueTask<T>` y `Task<T>`, y cuándo debería preferir uno sobre el otro?
**Ideal Answer:** `Task<T>` siempre asigna un objeto en el heap. `ValueTask<T>` es un struct que evita la asignación cuando el resultado ya está disponible de forma sincrónica (por ejemplo, valores en caché, rutas frecuentes). Prefiera `ValueTask<T>` para APIs de alto rendimiento donde la operación frecuentemente se completa de forma sincrónica. Evítelo cuando el resultado siempre es asincrónico, ya que esperar o almacenarlo repetidamente es inseguro y puede provocar comportamiento indefinido. `Task<T>` es más seguro para uso general; `ValueTask<T>` es una optimización.

## Q2
**Category:** Lenguaje C#
**Difficulty:** Hard
**Question:** Explique cómo funciona `IAsyncEnumerable<T>` y describa un caso de uso real donde es preferible a devolver un `List<T>` o `IEnumerable<T>`.
**Ideal Answer:** `IAsyncEnumerable<T>` permite la iteración asincrónica usando `await foreach`. Los elementos se producen uno a la vez a medida que están disponibles, sin almacenar en búfer toda la colección. Es ideal para transmitir grandes conjuntos de resultados desde una base de datos (por ejemplo, leer miles de filas con `QueryUnbufferedAsync` de Dapper), transmitir respuestas de API o leer de colas de mensajes. Devolver un `List<T>` materializa todo en memoria primero; `IAsyncEnumerable<T>` reduce la presión de memoria y el tiempo hasta el primer resultado.

## Q3
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** ¿Qué son los constructores primarios en C# 12, y cuáles son las ventajas y desventajas de usarlos en clases vs. records?
**Ideal Answer:** Los constructores primarios permiten declarar parámetros directamente en la declaración de la clase o struct. En records han estado disponibles desde C# 9 y generan propiedades públicas de solo inicialización automáticamente. En clases (C# 12), los parámetros están en alcance a lo largo del cuerpo de la clase pero no se promueven automáticamente a propiedades — se capturan como campos privados si se referencian. Esto significa que pueden ser mutados a menos que se tenga cuidado. Ventaja/desventaja: reducen el código repetitivo pero pueden ser menos explícitos sobre si un parámetro se convierte en propiedad o campo, lo que puede afectar la legibilidad.

## Q4
**Category:** Lenguaje C#
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre `Span<T>`, `Memory<T>` y `ArraySegment<T>`? ¿Cuándo es apropiado cada uno?
**Ideal Answer:** `Span<T>` es un ref struct solo de pila que proporciona una ventana sobre memoria contigua (array, búfer asignado en pila o memoria nativa) sin asignación en el heap. No puede almacenarse en campos ni usarse a través de límites async. `Memory<T>` es la contraparte compatible con el heap de `Span<T>`, seguro para almacenar en campos y usar con código async. `ArraySegment<T>` es el predecesor más antiguo y menos eficiente — solo funciona con arrays y tiene más sobrecarga. Use `Span<T>` para análisis sincrónico, confinado a la pila y de alto rendimiento; use `Memory<T>` para escenarios async; evite `ArraySegment<T>` en código nuevo.

## Q5
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** Explique el tipo `record` en C#. ¿Qué genera el compilador, y qué es la igualdad estructural vs. referencial?
**Ideal Answer:** Un `record` es un tipo por referencia (o `record struct` para tipo por valor) donde el compilador genera: `Equals` y `GetHashCode` basados en valores comparando todas las propiedades, un `ToString` que imprime los valores de las propiedades, un constructor de copia y un método de deconstrucción. La igualdad estructural significa que dos instancias con valores de propiedades idénticos se consideran iguales, a diferencia de las clases que usan igualdad referencial (misma dirección de memoria). Los records también soportan mutación no destructiva mediante expresiones `with`. Son ideales para DTOs y modelos de dominio inmutables.

## Q6
**Category:** Lenguaje C#
**Difficulty:** Hard
**Question:** ¿Qué son los generadores de código fuente, y cómo difieren de los analizadores de Roslyn y las plantillas T4?
**Ideal Answer:** Los generadores de código fuente se ejecutan durante la compilación y emiten archivos fuente C# adicionales en la compilación. A diferencia de las plantillas T4 (que se ejecutan en tiempo de diseño fuera del pipeline del compilador), los generadores de código fuente tienen acceso completo al modelo semántico a través de las APIs de Roslyn y pueden inspeccionar el código que se está compilando. A diferencia de los analizadores (que solo reportan diagnósticos), los generadores producen código nuevo. Casos de uso incluyen eliminar la serialización basada en reflexión (por ejemplo, generador de código fuente de `System.Text.Json`), generar código de mapeo o producir accesores de configuración fuertemente tipados. Mejoran el rendimiento de inicio y la compatibilidad con AOT.

## Q7
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre covarianza y contravarianza en genéricos? Dé un ejemplo práctico.
**Ideal Answer:** La covarianza (`out T`) permite usar un tipo más derivado donde se espera un tipo base, válido para productores (por ejemplo, `IEnumerable<string>` puede asignarse a `IEnumerable<object>`). La contravarianza (`in T`) permite un tipo más general donde se espera uno derivado, válido para consumidores (por ejemplo, `Action<object>` puede asignarse a `Action<string>`). Solo las interfaces y delegados soportan varianza. Un ejemplo práctico: `IEnumerable<T>` es covariante, por lo que `IEnumerable<Dog>` es asignable a `IEnumerable<Animal>` — seguro porque solo se lee y nunca se escribe.

## Q8
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre `AddScoped`, `AddTransient` y `AddSingleton` en la inyección de dependencias de ASP.NET Core, y qué problemas pueden surgir de su mal uso?
**Ideal Answer:** `Singleton` crea una instancia para toda la vida de la aplicación. `Scoped` crea una por solicitud HTTP. `Transient` crea una nueva instancia cada vez que se solicita. Un error común es "dependencias cautivas": inyectar un servicio scoped o transient en un singleton significa que el servicio de vida corta se promueve efectivamente a tiempo de vida singleton, provocando estado obsoleto o condiciones de carrera. El `DbContext` de EF Core es scoped y nunca debe inyectarse en un singleton. ASP.NET Core lanzará una excepción al inicio si esto se detecta con la validación de alcance habilitada.

## Q9
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** ¿Cómo difiere el middleware de los filtros en ASP.NET Core, y cuándo usaría cada uno?
**Ideal Answer:** El middleware opera a nivel del pipeline HTTP y se ejecuta para cada solicitud independientemente del enrutamiento. Maneja preocupaciones transversales como logging, CORS, autenticación y manejo de excepciones. Los filtros operan dentro del pipeline de acciones de MVC/minimal API y tienen acceso al contexto de la acción, el resultado y el estado del modelo — solo se ejecutan para solicitudes que llegan a un endpoint. Use middleware para preocupaciones que aplican globalmente a todas las solicitudes. Use filtros para preocupaciones vinculadas a acciones de controladores (por ejemplo, autorización en endpoints específicos, logging a nivel de acción, transformación de respuestas).

## Q10
**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** Explique cómo se comparan las minimal APIs con las APIs basadas en controladores en ASP.NET Core. ¿Cuáles son las implicaciones de rendimiento?
**Ideal Answer:** Las minimal APIs tienen menor sobrecarga que las APIs basadas en controladores porque omiten el pipeline completo de MVC: sin enlace de modelo mediante reflexión en controladores, sin pipeline de filtros de acción a menos que se agreguen explícitamente, y un modelo de enrutamiento más simple. Resultan en tiempos de arranque en frío más rápidos y mejor compatibilidad con AOT. Las APIs basadas en controladores proporcionan más convenciones integradas (enrutamiento por atributos, filtros de acción, comportamiento de `[ApiController]`). Para microservicios de alto rendimiento, se prefieren las minimal APIs. Para APIs grandes, mantenidas por equipos con políticas complejas de autorización y validación, los controladores con filtros pueden ser más mantenibles.

## Q11
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** ¿Qué es `IOptions<T>`, `IOptionsSnapshot<T>` e `IOptionsMonitor<T>`? ¿Cuándo usaría cada uno?
**Ideal Answer:** `IOptions<T>` es singleton — la configuración se lee una vez al inicio y nunca cambia. `IOptionsSnapshot<T>` es scoped — relee la configuración por solicitud, útil en aplicaciones web donde la configuración puede cambiar. `IOptionsMonitor<T>` es singleton que soporta notificaciones de cambio y callbacks `OnChange`, ideal para servicios en segundo plano que necesitan reaccionar a cambios de configuración en tiempo de ejecución. Use `IOptions<T>` para configuración estática, `IOptionsSnapshot<T>` cuando se necesita frescura por solicitud, e `IOptionsMonitor<T>` para servicios de larga ejecución.

## Q12
**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** ¿Cómo maneja el pipeline de solicitudes de ASP.NET Core las excepciones? Compare `UseExceptionHandler`, `app.UseStatusCodePages` y un enfoque de middleware personalizado.
**Ideal Answer:** `UseExceptionHandler` captura excepciones no manejadas lanzadas en cualquier parte del pipeline y re-ejecuta en la ruta de error (por ejemplo, `/error`). Es el enfoque estándar en producción. `UseStatusCodePages` agrega cuerpos de respuesta para códigos de estado sin cuerpo (por ejemplo, 404, 405) — no captura excepciones. Un middleware personalizado que envuelve `next()` en un try/catch da el mayor control: puede producir una respuesta `ProblemDetails` consistente, registrar con un ID de correlación y evitar la sobrecarga de re-ejecución de `UseExceptionHandler`. La desventaja del middleware personalizado es una mayor responsabilidad de mantenimiento.

## Q13
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** ¿Cuál es el rol de `IHostedService` y `BackgroundService`? ¿Cómo implementaría un worker en segundo plano confiable?
**Ideal Answer:** `IHostedService` define los hooks de ciclo de vida `StartAsync` y `StopAsync`. `BackgroundService` es una clase base que simplifica esto exponiendo `ExecuteAsync`. Para un worker confiable: sobrescriba `ExecuteAsync`, ejecute un bucle con un `CancellationToken`, use `Task.Delay` o un `PeriodicTimer` para intervalos, capture todas las excepciones para evitar que el host se apague, e implemente un apagado graceful respetando la cancelación. Para trabajo durable (por ejemplo, procesar colas), prefiera usar un broker de mensajes con entrega al-menos-una-vez en lugar de colas en memoria.

## Q14
**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** ¿Qué es el problema de consultas N+1 en EF Core, y cómo lo resuelve?
**Ideal Answer:** N+1 ocurre cuando se carga una lista de N entidades y luego se carga de forma lazy una entidad relacionada para cada una, resultando en N+1 viajes de ida y vuelta a la base de datos. Soluciones en EF Core: (1) Carga eager con `.Include()` / `.ThenInclude()` — obtiene datos relacionados en la misma consulta usando JOINs. (2) Carga explícita — carga controlada después del hecho. (3) Consultas divididas con `.AsSplitQuery()` — ejecuta consultas separadas por navegación pero evita la explosión cartesiana en colecciones. Evite la carga lazy en APIs de producción ya que genera silenciosamente consultas excesivas.

## Q15
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre el seguimiento de `SaveChanges` y `AsNoTracking`? ¿Cuándo usaría cada uno?
**Ideal Answer:** Por defecto, EF Core rastrea las entidades devueltas por consultas en el `ChangeTracker`. En `SaveChanges`, detecta cambios y genera sentencias UPDATE. `AsNoTracking()` omite el rastreo — las entidades no se registran en el change tracker, haciendo las lecturas más rápidas (menos memoria, sin comparación de snapshots). Use rastreo cuando pretenda modificar y guardar la entidad. Use `AsNoTracking` para consultas de solo lectura (por ejemplo, endpoints GET, modelos de lectura tipo Dapper) para mejorar el rendimiento. `AsNoTrackingWithIdentityResolution` es un punto intermedio: sin rastreo pero resolviendo la identidad de objetos.

## Q16
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** ¿Cómo maneja EF Core la concurrencia optimista? ¿Cuál es el enfoque de la columna del sistema `xmin` en PostgreSQL?
**Ideal Answer:** La concurrencia optimista previene actualizaciones perdidas verificando que una fila no haya cambiado desde que fue leída. EF Core marca una propiedad como token de concurrencia con `[ConcurrencyCheck]` o `.IsRowVersion()`. En UPDATE/DELETE, EF agrega una cláusula WHERE sobre ese token; si no se afectan filas, lanza `DbUpdateConcurrencyException`. En SQL Server, `rowversion` se auto-genera. En PostgreSQL, el enfoque `[Timestamp] byte[]` no funciona porque `bytea` no se auto-genera. El enfoque correcto en PostgreSQL usa la columna del sistema `xmin` (tipo `xid`, mapeado a `uint` en C#) mediante `.HasColumnName("xmin").HasColumnType("xid").IsRowVersion()` — Postgres actualiza automáticamente `xmin` en cada escritura.

## Q17
**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre code-first y database-first en EF Core? ¿Cuáles son los pros y contras de cada uno?
**Ideal Answer:** Code-first: se define el modelo en C#, EF genera migraciones y el esquema. Pros: control total del esquema en el control de versiones, amigable para refactorización, migraciones como código. Contras: esquemas complejos (por ejemplo, procedimientos almacenados, indexación avanzada) requieren ediciones manuales de migraciones. Database-first: se genera el modelo a partir de una base de datos existente usando `dotnet ef dbcontext scaffold`. Pros: útil al unirse a una BD existente. Contras: el código generado es verboso, difícil de personalizar, y diverge de la BD a medida que ambos evolucionan. Para proyectos nuevos, code-first es fuertemente preferido.

## Q18
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** ¿Cómo implementaría un patrón de eliminación lógica (soft delete) en EF Core sin modificar cada consulta en la base de código?
**Ideal Answer:** Use un filtro de consulta global: defina una columna `IsDeleted` en la entidad y configure `.HasQueryFilter(e => !e.IsDeleted)` en la configuración de la entidad. EF Core automáticamente agrega `WHERE is_deleted = false` a todas las consultas para esa entidad. Para eliminar permanentemente, use `.IgnoreQueryFilters()`. Combine con una sobrescritura de `SaveChanges` que intercepte `EntityState.Deleted`, establezca `IsDeleted = true` y `DeletedAt = now`, y cambie el estado a `Modified`. Esto hace la eliminación lógica transparente en toda la base de código.

## Q19
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre CQRS y una arquitectura de capas tradicional? ¿Qué problemas resuelve CQRS?
**Ideal Answer:** En una arquitectura de capas, el mismo modelo sirve para lecturas y escrituras, lo que frecuentemente lleva a agregados complejos y sobre-obtención de datos. CQRS (Segregación de Responsabilidad de Comandos y Consultas) separa el modelo de escritura (comandos que cambian estado) del modelo de lectura (consultas que devuelven datos). Beneficios: los modelos de lectura pueden optimizarse independientemente (por ejemplo, vistas desnormalizadas, Dapper en lugar de EF), los comandos pueden imponer invariantes en un modelo de dominio rico, y cada lado puede escalar independientemente. También se combina naturalmente con Event Sourcing. Desventaja: más código, consistencia eventual en escenarios distribuidos.

## Q20
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Qué es el Patrón Outbox, y cómo resuelve el problema de doble escritura en sistemas distribuidos?
**Ideal Answer:** El problema de doble escritura: guardar en una base de datos y publicar un evento en un broker de mensajes en dos operaciones separadas puede dejarlos inconsistentes si una falla. El Patrón Outbox resuelve esto escribiendo tanto el cambio de dominio como el evento en la misma transacción de base de datos (el evento va a una tabla `outbox`). Un proceso en segundo plano separado consulta el outbox y publica eventos en el broker, luego los marca como publicados. Esto garantiza entrega al-menos-una-vez sin eventos perdidos. Herramientas como MassTransit (con outbox de EF Core) o Debezium (basado en CDC) implementan este patrón.

## Q21
**Category:** Arquitectura
**Difficulty:** Medium
**Question:** Explique el patrón Repository. ¿Cuándo agrega valor, y cuándo agrega abstracción innecesaria?
**Ideal Answer:** El patrón Repository abstrae el acceso a datos detrás de una interfaz, habilitando la testabilidad (simular el repositorio) y desacoplando el dominio de la capa de datos. Agrega valor cuando: es probable que la tecnología de persistencia cambie, la lógica de consultas complejas necesita centralizarse, o se requieren pruebas unitarias sin una base de datos real. Agrega abstracción innecesaria cuando: el `DbContext` de EF Core (que ya es una unidad de trabajo + repositorio) se envuelve en un repositorio de paso delgado, agregando código repetitivo sin beneficio. En CQRS, los manejadores de comandos frecuentemente usan `DbContext` directamente, mientras que los modelos de lectura usan manejadores de consultas ligeros (por ejemplo, Dapper), haciendo redundante un repositorio genérico.

## Q22
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Cuáles son las ventajas y desventajas entre una arquitectura de microservicios y un monolito modular?
**Ideal Answer:** Microservicios: desplegabilidad independiente, heterogeneidad tecnológica, aislamiento de fallos, escalado horizontal por servicio. Costos: latencia de red, transacciones distribuidas, descubrimiento de servicios, complejidad operacional (Kubernetes, service mesh, rastreo distribuido). Monolito modular: mismo límite de despliegue pero con límites de módulos internos forzados (ensamblados separados, sin acceso directo a BD entre módulos). Más fácil de desarrollar, probar y desplegar. Menor sobrecarga operacional. Punto de partida recomendado para la mayoría de equipos — puede extraer servicios cuando un límite está comprobado y la necesidad de escalado es real. La descomposición prematura en microservicios es uno de los errores arquitectónicos más comunes.

## Q23
**Category:** Arquitectura
**Difficulty:** Medium
**Question:** ¿Qué es la Arquitectura de Rebanadas Verticales (Vertical Slice), y cómo difiere de una arquitectura de capas tradicional (N-tier)?
**Ideal Answer:** En N-tier (Controladores → Servicios → Repositorios → BD), cada funcionalidad toca todas las capas horizontalmente. Vertical Slice organiza el código por funcionalidad: cada funcionalidad posee su comando/consulta, manejador y acceso a datos en una carpeta. Beneficios: menor acoplamiento entre funcionalidades, los cambios se aíslan en una sola rebanada, más fácil de incorporar nuevos desarrolladores (encuentra todo de una funcionalidad en un lugar), y cada rebanada puede elegir su propio patrón (algunas pueden usar EF, otras SQL crudo). La desventaja es la posible duplicación de código entre rebanadas y la necesidad de disciplina para evitar que el estado compartido se infiltre.

## Q24
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Cómo diseñaría un endpoint de API idempotente? ¿Por qué es importante la idempotencia, y cómo se implementa típicamente?
**Ideal Answer:** Una operación idempotente produce el mismo resultado independientemente de cuántas veces se llame. GET, PUT y DELETE son naturalmente idempotentes; POST no lo es. Para hacer un POST idempotente: requiera que los clientes envíen un encabezado `Idempotency-Key` (un UUID). El servidor almacena la clave y la respuesta en una caché o BD. En una solicitud duplicada con la misma clave, devuelve la respuesta almacenada sin re-ejecutar. Esto es crítico para transacciones financieras, creación de pedidos y cualquier operación con efectos secundarios — permite reintentos seguros después de fallas de red sin duplicar estado.

## Q25
**Category:** Arquitectura
**Difficulty:** Medium
**Question:** ¿Qué es el Diseño Dirigido por Dominio (DDD)? Explique los conceptos de Agregado, Entidad, Objeto de Valor y Contexto Delimitado.
**Ideal Answer:** DDD es un enfoque de diseño de software que alinea la estructura del código con el dominio de negocio. Entidad: tiene una identidad única (por ejemplo, `Order` identificado por `OrderId`). Objeto de Valor: definido por sus atributos, sin identidad, inmutable (por ejemplo, `Money`, `Address`). Agregado: un grupo de entidades y objetos de valor con una entidad raíz que impone invariantes — todo acceso pasa por la raíz. Contexto Delimitado: un límite explícito dentro del cual un modelo de dominio es consistente y aplica un lenguaje ubicuo. Diferentes contextos pueden modelar el mismo concepto de manera diferente (por ejemplo, "Cliente" en Facturación vs. Envío). Los Contextos Delimitados se comunican mediante interfaces bien definidas (eventos, APIs).

## Q26
**Category:** Rendimiento
**Difficulty:** Hard
**Question:** ¿Cómo diagnosticaría y solucionaría un endpoint de API lento en una aplicación .NET en producción?
**Ideal Answer:** Comience con la observabilidad: revise las trazas distribuidas (por ejemplo, OpenTelemetry + Jaeger) para identificar dónde se gasta el tiempo. Revise la duración de las consultas a la base de datos — las consultas N+1 y los índices faltantes son las causas más comunes. Use `EXPLAIN ANALYZE` en consultas lentas. Verifique las asignaciones de memoria con dotMemory o `dotnet-counters` (la presión del GC puede causar picos de latencia). Perfile la CPU con `dotnet-trace` y analice con PerfView o SpeedScope. En código, busque I/O sincrónico bloqueando hilos async, serialización excesiva o asignaciones de objetos grandes. Corrija en orden: consultas primero (índices, proyecciones), luego asignaciones, luego complejidad algorítmica.

## Q27
**Category:** Rendimiento
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre `IMemoryCache` e `IDistributedCache` en ASP.NET Core? ¿Cuándo usaría cada uno?
**Ideal Answer:** `IMemoryCache` almacena datos en la memoria del proceso — rápido, sin latencia, pero no compartido entre múltiples instancias. Adecuado para aplicaciones de una sola instancia o datos que pueden diferir por instancia (por ejemplo, limitación de tasa local). `IDistributedCache` usa un almacén externo (Redis, SQL Server) compartido entre todas las instancias — datos consistentes para despliegues escalados. Use `IMemoryCache` para escenarios simples de un solo servidor. Use `IDistributedCache` (respaldado por Redis) en Kubernetes o despliegues multi-instancia donde todos los nodos deben ver los mismos datos en caché (por ejemplo, estado de sesión, limitación de tasa distribuida, búsquedas compartidas).

## Q28
**Category:** Rendimiento
**Difficulty:** Hard
**Question:** ¿Qué es el problema de agotamiento del ThreadPool en código async de .NET, y cómo se manifiesta?
**Ideal Answer:** El agotamiento del ThreadPool ocurre cuando todos los hilos disponibles están bloqueados esperando que operaciones async se completen (típicamente debido a `.Result` o `.Wait()` en métodos async, o I/O sincrónico). El trabajo nuevo no puede ejecutarse porque no hay hilos libres, causando timeouts y solicitudes encoladas. Síntomas: alta latencia de solicitudes bajo carga, bajo uso de CPU, conteo de hilos creciente. Solución: siempre use `await` y nunca bloquee código async. Sea especialmente cauteloso en middleware, filtros y código de bibliotecas. Use `async` a lo largo de toda la pila de llamadas. Monitoree con `dotnet-counters` observando `ThreadPool Queue Length` y `ThreadPool Thread Count`.

## Q29
**Category:** Rendimiento
**Difficulty:** Medium
**Question:** ¿Cómo difiere `System.Text.Json` de `Newtonsoft.Json`, y cuáles son las ventajas y desventajas de rendimiento?
**Ideal Answer:** `System.Text.Json` es la biblioteca JSON integrada de Microsoft, diseñada para rendimiento: menos asignaciones, análisis basado en `Span<byte>` y soporte de generador de código fuente para AOT. Es significativamente más rápido que Newtonsoft.Json para escenarios comunes. Sin embargo, tiene menos funcionalidades: sin manipulación `JObject`/JSON dinámico, deserialización más estricta por defecto (sin coincidencia insensible a mayúsculas por defecto, sin manejo de miembros faltantes), y soporte limitado para escenarios complejos como polimorfismo sin convertidores personalizados. Newtonsoft.Json es más flexible y probado en batalla para escenarios complejos. Para servicios nuevos, prefiera `System.Text.Json` con generadores de código fuente; recurra a Newtonsoft.Json solo para requisitos de serialización complejos.

## Q30
**Category:** Seguridad
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre autenticación y autorización en ASP.NET Core? ¿Cómo funciona la autenticación basada en JWT?
**Ideal Answer:** La autenticación establece quién es el usuario. La autorización determina qué se le permite hacer. En ASP.NET Core, `UseAuthentication()` se ejecuta primero y puebla `HttpContext.User` basándose en las credenciales proporcionadas. `UseAuthorization()` luego verifica los claims del usuario contra políticas o atributos `[Authorize]`. Autenticación JWT: el cliente envía un token firmado en el encabezado `Authorization: Bearer`. El middleware valida la firma usando la clave secreta del servidor, verifica la expiración y claims de emisor/audiencia, y puebla `ClaimsPrincipal`. No se necesita sesión del lado del servidor — el token es autocontenido. Expiración corta + tokens de refresco es el patrón recomendado.

## Q31
**Category:** Seguridad
**Difficulty:** Hard
**Question:** ¿Cuáles son los riesgos del OWASP Top 10 más relevantes para APIs backend .NET? ¿Cómo los mitigaría?
**Ideal Answer:** Riesgos clave para APIs .NET: (1) Control de Acceso Roto — use `[Authorize]` con verificaciones a nivel de recurso, nunca confíe solo en IDs proporcionados por el cliente. (2) Inyección (SQL, comandos) — use consultas parametrizadas (EF Core, Dapper), nunca concatene entrada de usuario en SQL. (3) Configuración de Seguridad Incorrecta — deshabilite Swagger en producción, use HTTPS, valide estrictamente los parámetros JWT. (4) Exposición de Datos Sensibles — nunca registre tokens/contraseñas, cifre PII en reposo. (5) Autenticación Rota — tiempos de vida JWT cortos, rotación de tokens de refresco, revocación al cerrar sesión. (6) SSRF — valide y use lista de permitidos para cualquier URL que el servidor obtenga. (7) Asignación Masiva — use DTOs en lugar de vincular directamente a entidades de dominio.

## Q32
**Category:** Seguridad
**Difficulty:** Medium
**Question:** ¿Qué es la rotación de tokens de refresco, y por qué es importante para los flujos OAuth?
**Ideal Answer:** La rotación de tokens de refresco significa emitir un nuevo token de refresco cada vez que el actual se usa para obtener un nuevo token de acceso, e invalidar el anterior. Esto limita la ventana de exposición si un token de refresco es robado — el atacante solo puede usarlo una vez antes de que sea rotado. Implemente familias de tokens de refresco: si se presenta un token antiguo (ya rotado), esto señala un posible robo — invalide toda la familia. Los tokens de refresco deben almacenarse hasheados en la base de datos (no en texto plano), tener una expiración larga pero limitada (por ejemplo, 30 días), y estar vinculados a un identificador de dispositivo/sesión para seguridad adicional.

## Q33
**Category:** Seguridad
**Difficulty:** Hard
**Question:** ¿Cómo prevendría la inyección SQL al usar Dapper?
**Ideal Answer:** Siempre use consultas parametrizadas. En Dapper, pase parámetros como un objeto anónimo: `db.QueryAsync<T>("SELECT * FROM users WHERE id = @Id", new { Id = id })`. Nunca use interpolación de cadenas o concatenación para construir SQL: `$"SELECT * FROM users WHERE id = {id}"` es vulnerable. Para ORDER BY dinámicos o nombres de tabla (que no pueden parametrizarse), use una lista blanca de valores permitidos validados en código. Dapper no proporciona ningún escape automático — el desarrollador es completamente responsable de la parametrización. También evite el método `Execute` con SQL de múltiples sentencias proporcionado por el usuario.

## Q34
**Category:** Pruebas
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre pruebas unitarias, pruebas de integración y pruebas de extremo a extremo? ¿Cómo decide el balance correcto para una API .NET?
**Ideal Answer:** Pruebas unitarias: prueban una sola clase o función de forma aislada, con todas las dependencias simuladas. Rápidas, sin I/O. Pruebas de integración: prueban múltiples componentes juntos incluyendo infraestructura real (base de datos, cliente HTTP). Más lentas pero detectan errores reales de interacción. Pruebas de extremo a extremo: prueban el sistema completo desde la perspectiva del cliente. Las más lentas y frágiles. Para APIs .NET, el balance recomendado (pirámide de pruebas): muchas pruebas unitarias para lógica de negocio (manejadores de comandos/consultas, servicios de dominio); pruebas de integración para consultas a BD, middleware y comportamiento de endpoints usando `WebApplicationFactory`; pocas pruebas de extremo a extremo solo para recorridos críticos del usuario.

## Q35
**Category:** Pruebas
**Difficulty:** Medium
**Question:** ¿Cómo funciona `WebApplicationFactory<T>` en pruebas de integración de ASP.NET Core, y cómo reemplaza servicios para pruebas?
**Ideal Answer:** `WebApplicationFactory<T>` crea un servidor de pruebas usando el inicio real de `Program.cs`, permitiendo pruebas de integración HTTP completas sin desplegar la aplicación. Levanta un `TestServer` en memoria y proporciona un `HttpClient` para solicitudes. Para reemplazar servicios, sobrescriba `ConfigureWebHost` y llame a `builder.ConfigureServices(services => services.AddSingleton<IMyService>(mock))`. Para bases de datos, reemplace las opciones del `DbContext` con una base de datos en memoria o de test-container. Este enfoque prueba el pipeline completo incluyendo enrutamiento, middleware, autenticación y serialización, haciéndolo el tipo de prueba más valioso para la corrección de la API.

## Q36
**Category:** Pruebas
**Difficulty:** Hard
**Question:** ¿Cuáles son los riesgos de usar bases de datos simuladas en pruebas de integración, y cuál es la alternativa recomendada?
**Ideal Answer:** Las bases de datos simuladas o en memoria (por ejemplo, `UseInMemoryDatabase`) no imponen restricciones SQL, no prueban el rendimiento de consultas y difieren significativamente de una base de datos real en comportamiento (sin transacciones, sin migraciones, sin validaciones a nivel SQL). Los errores que solo se manifiestan contra Postgres o SQL Server reales no serán detectados. La alternativa recomendada es Testcontainers: levantar un contenedor Docker real de la base de datos objetivo por ejecución de pruebas. La biblioteca `Testcontainers.PostgreSql` (o `MsSql`) maneja el ciclo de vida del contenedor. Las pruebas se ejecutan contra una base de datos real, las migraciones se aplican y el contenedor se destruye después de la suite de pruebas. Esto proporciona alta confianza sin requerir una base de datos de pruebas compartida.

## Q37
**Category:** Pruebas
**Difficulty:** Medium
**Question:** ¿Qué es FluentAssertions, y cómo mejora la legibilidad de las pruebas comparado con las aserciones integradas de xUnit?
**Ideal Answer:** FluentAssertions proporciona una API fluida de lenguaje natural para aserciones. En lugar de `Assert.Equal(expected, actual)`, se escribe `actual.Should().Be(expected)` o `result.Should().HaveCount(3).And.Contain(x => x.Name == "Test")`. Cuando una prueba falla, FluentAssertions produce mensajes de error detallados y legibles mostrando los valores reales vs. esperados incluyendo la estructura del objeto. Soporta aserciones ricas para colecciones, excepciones (`action.Should().Throw<ArgumentException>()`), fechas, cadenas y código async. Reduce significativamente el tiempo dedicado a depurar pruebas fallidas.

## Q38
**Category:** Azure
**Difficulty:** Medium
**Question:** ¿Qué es Azure Service Bus, y cómo difiere de Azure Storage Queues?
**Ideal Answer:** Azure Service Bus es un broker de mensajes empresarial que soporta temas/suscripciones (pub-sub), sesiones de mensajes (procesamiento ordenado), colas de mensajes no entregados, detección de duplicados, transacciones y entrega al-menos-una-vez. Storage Queues son más simples, más económicas y tienen mayor rendimiento pero carecen de temas, sesiones y transacciones. Use Service Bus cuando necesite: distribución pub-sub, ordenamiento de mensajes, colas de mensajes no entregados con políticas de reintento, o mensajería transaccional. Use Storage Queues para escenarios simples, de alto volumen y sensibles al costo donde no se necesitan funcionalidades avanzadas de mensajería. Service Bus se integra bien con MassTransit en .NET.

## Q39
**Category:** Azure
**Difficulty:** Hard
**Question:** ¿Qué es Azure Managed Identity, y cómo reemplaza las cadenas de conexión para autenticarse en recursos de Azure?
**Ideal Answer:** Managed Identity proporciona una identidad de Azure AD a un recurso de Azure (por ejemplo, App Service, pod de AKS) sin almacenar credenciales. Asignada por el sistema: vinculada al ciclo de vida del recurso. Asignada por el usuario: independiente, puede compartirse entre recursos. En lugar de una cadena de conexión con usuario/contraseña, la aplicación usa `DefaultAzureCredential` del SDK de Azure, que automáticamente resuelve el token de identidad del entorno. Ejemplo: conectarse a Azure SQL con `Authentication=Active Directory Managed Identity` en la cadena de conexión, o conectarse a Key Vault usando `new SecretClient(uri, new DefaultAzureCredential())`. Elimina secretos de archivos de configuración y la carga de rotación.

## Q40
**Category:** Azure
**Difficulty:** Medium
**Question:** ¿Qué es Azure Key Vault, y cómo lo integra con el sistema de configuración de una aplicación ASP.NET Core?
**Ideal Answer:** Azure Key Vault almacena secretos, certificados y claves de cifrado con control de acceso granular y registros de auditoría. Integración con ASP.NET Core: agregue el paquete NuGet `Azure.Extensions.AspNetCore.Configuration.Secrets` y llame a `builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential())`. Los secretos de Key Vault están entonces disponibles a través de la interfaz estándar `IConfiguration`. Los nombres de secretos usan `--` como separador para configuración anidada (por ejemplo, `ConnectionStrings--Default`). Combine con Managed Identity para que no se necesiten credenciales para acceder al vault en sí. En desarrollo, use `dotnet user-secrets` o un archivo `.env` local.

## Q41
**Category:** Azure
**Difficulty:** Hard
**Question:** ¿Cómo complementa Azure API Management (APIM) a una API backend .NET? ¿Qué funcionalidades proporciona?
**Ideal Answer:** APIM se sitúa frente a las APIs backend y proporciona: limitación de tasa y throttling por suscripción/producto, autenticación (validación JWT, OAuth, certificados de cliente) antes de que las solicitudes lleguen al backend, transformación de solicitud/respuesta mediante políticas (agregar encabezados, reescribir URLs), caché, registro en Application Insights, versionado y un portal de desarrolladores para documentación de API. Beneficios: descargar preocupaciones transversales del backend, proteger servicios del abuso y proporcionar un punto de entrada unificado para múltiples servicios backend. En microservicios .NET, APIM actúa como un API gateway, reduciendo la duplicación de código de preocupaciones comunes entre servicios.

## Q42
**Category:** Azure
**Difficulty:** Medium
**Question:** ¿Qué es Azure Application Insights, y cómo lo configura para rastreo distribuido en una aplicación .NET?
**Ideal Answer:** Application Insights es el servicio APM de Azure. Para .NET, agregue `Microsoft.ApplicationInsights.AspNetCore` o use OpenTelemetry con el exportador de Azure Monitor. Configure con `builder.Services.AddApplicationInsightsTelemetry()` y establezca la variable de entorno `APPLICATIONINSIGHTS_CONNECTION_STRING`. Captura: telemetría de solicitudes, llamadas a dependencias (HTTP, SQL), excepciones, eventos personalizados y métricas. Para rastreo distribuido, usa el encabezado W3C `traceparent` para correlacionar telemetría entre servicios. El Mapa de Aplicación muestra las dependencias de servicios. Use `TelemetryClient` para eventos personalizados o el SDK de OpenTelemetry para instrumentación neutral de proveedor.

## Q43
**Category:** Azure
**Difficulty:** Hard
**Question:** ¿Cuáles son las opciones de despliegue para una API .NET en Azure? Compare Azure App Service, Azure Container Apps y AKS.
**Ideal Answer:** App Service: PaaS, modelo de despliegue más simple (zip deploy, GitHub Actions), autoescalado, SSL integrado, sin necesidad de conocimiento de orquestación de contenedores. Mejor para aplicaciones web sencillas. Azure Container Apps: contenedores serverless, autoescalado basado en eventos (KEDA), integración con Dapr, sin gestión de clústeres Kubernetes. Mejor para microservicios o aplicaciones containerizadas sin experiencia profunda en K8s. AKS (Azure Kubernetes Service): control completo de Kubernetes, cargas de trabajo complejas, redes personalizadas, stateful sets. Mayor costo operacional y complejidad. Mejor cuando se necesita planificación avanzada, operadores personalizados o aislamiento multi-tenant. Para la mayoría de APIs .NET: App Service para aplicaciones simples, Container Apps para microservicios container-first, AKS para plataformas complejas y a gran escala.

## Q44
**Category:** Azure
**Difficulty:** Medium
**Question:** ¿Qué es Azure Blob Storage, y cómo lo usa desde una aplicación .NET para subida de archivos?
**Ideal Answer:** Azure Blob Storage almacena datos no estructurados (archivos, imágenes, logs) en contenedores. Desde .NET, use el SDK `Azure.Storage.Blobs`: cree un `BlobServiceClient` (autenticado mediante cadena de conexión o `DefaultAzureCredential`), obtenga un `BlobContainerClient` y suba con `blobClient.UploadAsync(stream, overwrite: true)`. Para subidas de archivos grandes desde clientes, prefiera tokens SAS (Firma de Acceso Compartido): genere una URL con tiempo limitado en el servidor y deje que el cliente suba directamente a Blob Storage, evitando su API. Esto evita transmitir archivos grandes a través del servidor, reduciendo la presión de memoria y los costos de salida.

## Q45
**Category:** Azure
**Difficulty:** Hard
**Question:** ¿Qué es Azure Functions, y cuándo lo elegiría sobre un `BackgroundService` alojado en ASP.NET Core?
**Ideal Answer:** Azure Functions es un servicio de cómputo serverless que ejecuta código en respuesta a disparadores (HTTP, temporizador, Service Bus, Blob, Event Grid). Escala a cero (sin costo cuando está inactivo), escala automáticamente y no tiene gestión de servidores. Elija Functions cuando: la carga de trabajo es basada en eventos y esporádica, desea facturación por ejecución, o desea desplegabilidad aislada por función. Elija `BackgroundService` en ASP.NET Core cuando: necesita integración estrecha con el contenedor DI de la misma aplicación, el trabajo es continuo (por ejemplo, polling), o necesita latencia predecible sin arranque en frío. Los arranques en frío de Functions pueden mitigarse con el plan Premium o pre-calentamiento.

## Q46
**Category:** Bases de Datos
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre un índice agrupado y un índice no agrupado en bases de datos SQL? ¿Cómo afecta el diseño de índices al rendimiento de una API .NET?
**Ideal Answer:** Un índice agrupado define el orden físico de las filas en disco — hay uno por tabla (la clave primaria por defecto). Un índice no agrupado es una estructura separada con punteros a las filas reales. Las consultas que filtran u ordenan por columnas de índice no agrupado evitan escaneos completos de tabla. Para el rendimiento de API .NET: identifique consultas lentas mediante logging de EF Core o `EXPLAIN ANALYZE` (Postgres). Agregue índices no agrupados en columnas usadas en cláusulas WHERE, JOIN ON y ORDER BY. Los índices cubrientes (que incluyen todas las columnas seleccionadas) eliminan búsquedas por clave. El sobre-indexado perjudica el rendimiento de escritura. Las migraciones de EF Core soportan `.HasIndex()` e `.IncludeProperties()` para índices compuestos y cubrientes.

## Q47
**Category:** Bases de Datos
**Difficulty:** Medium
**Question:** ¿Qué es una transacción de base de datos, y cómo gestiona transacciones en EF Core y Dapper dentro de la misma unidad de trabajo?
**Ideal Answer:** Una transacción agrupa múltiples operaciones en una unidad atómica — todas tienen éxito o todas se revierten. En EF Core, `SaveChangesAsync()` envuelve todos los cambios pendientes en una transacción automáticamente. Para transacciones explícitas: `using var tx = await context.Database.BeginTransactionAsync()`. Para compartir una transacción entre EF Core y Dapper: obtenga la `DbConnection` y `DbTransaction` subyacentes de EF Core (`context.Database.GetDbConnection()`, `context.Database.CurrentTransaction.GetDbTransaction()`) y páselas a las sobrecargas de `Execute`/`Query` de Dapper. Esto asegura que tanto el ORM como el SQL crudo participen en la misma transacción.

## Q48
**Category:** Bases de Datos
**Difficulty:** Hard
**Question:** ¿Qué es el pool de conexiones en el contexto del acceso a bases de datos en .NET, y cómo funciona con Npgsql/PostgreSQL?
**Ideal Answer:** El pool de conexiones reutiliza conexiones establecidas a la base de datos en lugar de crear una nueva conexión TCP por solicitud, lo cual es costoso. Npgsql tiene un pool de conexiones integrado por cadena de conexión. Cuando se llama a `OpenAsync()`, Npgsql devuelve una conexión del pool; cuando se llama a `CloseAsync()` (o `Dispose()`), la devuelve al pool — sin cerrar realmente la conexión TCP. Configuraciones clave: `Maximum Pool Size` (predeterminado 100), `Minimum Pool Size`, `Connection Idle Lifetime`. Con el `DbContext` scoped de EF Core, cada solicitud obtiene una conexión del pool y la devuelve al final del alcance. Configurar incorrectamente el tamaño del pool en relación con `max_connections` de Postgres puede causar agotamiento de conexiones.

## Q49
**Category:** Mensajería
**Difficulty:** Hard
**Question:** ¿Qué es MassTransit, y cómo simplifica la comunicación basada en mensajes en microservicios .NET?
**Ideal Answer:** MassTransit es una abstracción de mensajería .NET de código abierto que soporta RabbitMQ, Azure Service Bus, Amazon SQS y otros mediante una API consistente. Maneja: registro de consumidores (integrado con DI), políticas de reintento, circuit breakers, colas de mensajes no entregados, sagas (orquestación de máquina de estados para flujos de trabajo distribuidos), solicitud/respuesta sobre mensajería, y el Patrón Outbox con EF Core. En lugar de escribir código específico del broker, se define `IConsumer<TMessage>` y se registra con `services.AddMassTransit(x => x.AddConsumer<MyConsumer>())`. Reduce significativamente el código repetitivo para mensajería confiable y se integra con los servicios alojados de ASP.NET Core.

## Q50
**Category:** Mensajería
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre mensajería punto a punto y publicación/suscripción? Dé un ejemplo concreto con Azure Service Bus.
**Ideal Answer:** Punto a punto (cola): un mensaje se envía a una cola nombrada y es consumido por exactamente un receptor. Adecuado para distribución de trabajo (por ejemplo, una cola de trabajos donde un worker procesa cada pedido). Publicación/suscripción (tema + suscripción): un publicador envía un mensaje a un tema; múltiples suscripciones (cada una con sus propias reglas de filtro) reciben copias independientes. Adecuado para integración basada en eventos (por ejemplo, evento `OrderPlaced` consumido por el servicio de Facturación y el servicio de Notificaciones independientemente). En Azure Service Bus: use `QueueClient` para P2P, use `TopicClient` + `SubscriptionClient` para pub-sub. MassTransit abstrae ambos patrones.

## Q51
**Category:** Patrones de Diseño
**Difficulty:** Medium
**Question:** ¿Qué es el patrón Mediador, y cómo lo implementa MediatR en ASP.NET Core?
**Ideal Answer:** El patrón Mediador centraliza la comunicación entre objetos a través de un único mediador, reduciendo el acoplamiento directo. MediatR lo implementa haciendo que todos los comandos y consultas implementen `IRequest<TResponse>`. Los manejadores implementan `IRequestHandler<TRequest, TResponse>` y son descubiertos mediante DI. El endpoint llama a `mediator.Send(new MyCommand(...))` sin saber qué manejador lo procesa. Esto desacopla la capa HTTP de la lógica de negocio, hace que los manejadores sean testables independientemente y soporta comportamientos transversales (pipeline behaviors) para logging, validación y caché insertados entre la solicitud y el manejador.

## Q52
**Category:** Patrones de Diseño
**Difficulty:** Medium
**Question:** ¿Qué es el patrón Decorador, y cómo puede aplicarse en .NET usando DI para agregar comportamiento transversal?
**Ideal Answer:** El Decorador envuelve un objeto para agregar comportamiento sin modificarlo. En DI de .NET, registre decoradores usando Scrutor (`services.Decorate<IMyService, LoggingMyService>()`) o manualmente resolviendo el servicio interno y envolviéndolo. Ejemplo: un `CachingQueryHandler<T>` que envuelve un manejador real y devuelve resultados en caché. Los pipeline behaviors en MediatR logran el mismo efecto para solicitudes. Beneficios: responsabilidad única (cada decorador hace una cosa), abierto/cerrado (agregar comportamiento sin modificar código existente), testable de forma aislada. Casos de uso comunes: logging, caché, reintento, verificaciones de autorización.

## Q53
**Category:** Patrones de Diseño
**Difficulty:** Hard
**Question:** ¿Qué es el patrón Specification, y cómo ayuda con la composición de consultas complejas en EF Core?
**Ideal Answer:** Una Specification encapsula criterios de consulta como un objeto. En lugar de pasar predicados crudos por todas partes, se define `class ActiveUserSpecification : Specification<User>` que construye un `Expression<Func<User, bool>>`. El repositorio o manejador de consultas aplica la specification: `context.Users.Where(spec.Criteria)`. Beneficios: reutilizable, combinable (operadores AND/OR), testable sin base de datos. Bibliotecas como Ardalis.Specification proporcionan una clase base e `ISpecificationEvaluator` que también maneja `Include`, `OrderBy` y paginación. Esto evita que la lógica de consultas se filtre a manejadores y controladores.

## Q54
**Category:** Patrones de Diseño
**Difficulty:** Medium
**Question:** ¿Qué es el patrón Factory? ¿Cuándo usaría `IServiceProvider` como factory vs. una clase factory dedicada?
**Ideal Answer:** El patrón Factory encapsula la creación de objetos. En .NET, `IServiceProvider.GetRequiredService<T>()` actúa como un localizador de servicios/factory pero es un antipatrón cuando se usa en lógica de negocio (oculta dependencias). Una interfaz factory dedicada (`INotificationFactory`) hace las dependencias explícitas y es más fácil de probar. Use `IServiceProvider` como factory solo en código de infraestructura (por ejemplo, crear servicios scoped dentro de un singleton como un servicio alojado: `scope = provider.CreateScope()`). Use una clase factory dedicada para crear objetos de dominio o cuando el tipo a crear depende de datos en tiempo de ejecución.

## Q55
**Category:** Logging y Observabilidad
**Difficulty:** Medium
**Question:** ¿Qué son los logs estructurados, y por qué son preferibles a los logs de texto plano en una aplicación cloud-native?
**Ideal Answer:** Los logs estructurados almacenan datos de log como pares clave-valor (JSON) en lugar de cadenas de forma libre. En lugar de `"El usuario 42 inició sesión desde 1.2.3.4"`, se emite `{ "event": "UserLogin", "userId": 42, "ip": "1.2.3.4" }`. Esto permite: filtrar y consultar por campos específicos (por ejemplo, todos los eventos para `userId = 42`), agregaciones y dashboards de métricas, y correlación de eventos entre servicios. En .NET, Serilog y Microsoft.Extensions.Logging soportan logging estructurado. Con Serilog, use plantillas de mensaje: `Log.Information("User {UserId} logged in from {IpAddress}", userId, ip)`. Los valores se capturan como propiedades estructuradas, no simplemente formateados en la cadena.

## Q56
**Category:** Logging y Observabilidad
**Difficulty:** Hard
**Question:** ¿Qué es OpenTelemetry, y cómo unifica trazas, métricas y logging en una aplicación .NET?
**Ideal Answer:** OpenTelemetry (OTel) es un framework de observabilidad neutral de proveedor que proporciona un único conjunto de APIs y SDKs para trazas, métricas y logs. En .NET: agregue `OpenTelemetry.Extensions.Hosting`, configure `TracerProvider` con `AddAspNetCoreInstrumentation()`, `AddHttpClientInstrumentation()`, `AddEntityFrameworkCoreInstrumentation()`, y exporte a Jaeger, Zipkin o Azure Monitor mediante `AddOtlpExporter()`. Las trazas se propagan mediante encabezados W3C `traceparent` entre servicios, habilitando la correlación de solicitudes de extremo a extremo. Las métricas se exportan a Prometheus. Esto reemplaza SDKs propietarios (SDK de Application Insights, trazador de Datadog) con una única capa de instrumentación, permitiendo cambiar backends sin cambios en el código.

## Q57
**Category:** Logging y Observabilidad
**Difficulty:** Medium
**Question:** ¿Qué es un ID de correlación, y cómo lo propagaría entre servicios y lo registraría de forma consistente?
**Ideal Answer:** Un ID de correlación es un identificador único adjunto a una solicitud y pasado a través de todas las llamadas de servicio y entradas de log, permitiendo rastrear una única solicitud de usuario a través de múltiples servicios y líneas de log. Implementación en ASP.NET Core: un middleware lee `X-Correlation-ID` de los encabezados entrantes (o genera uno nuevo), lo almacena en un servicio scoped o `Activity.Current`, y lo agrega a las solicitudes `HttpClient` salientes mediante un `DelegatingHandler`. En Serilog, use `LogContext.PushProperty("CorrelationId", id)` mediante enriquecimiento de `UseSerilogRequestLogging` para que cada línea de log en el alcance de esa solicitud incluya el ID. El `TraceId` de OpenTelemetry cumple el mismo propósito.

## Q58
**Category:** Diseño de API
**Difficulty:** Medium
**Question:** ¿Qué es ProblemDetails (RFC 7807), y cómo lo implementa de forma consistente en una API ASP.NET Core?
**Ideal Answer:** ProblemDetails es un formato estándar HTTP para respuestas de error legibles por máquina, incluyendo los campos `type`, `title`, `status`, `detail` e `instance`. En ASP.NET Core 7+, llame a `builder.Services.AddProblemDetails()` y use `Results.Problem(...)` en minimal APIs o `Problem(...)` en controladores. Para excepciones no manejadas, configure `UseExceptionHandler` para devolver `ProblemDetails` mediante `IExceptionHandler`. Esto da a los clientes un contrato de error consistente independientemente del endpoint, y permite propiedades de extensión (por ejemplo, `errors` para fallos de validación). Es preferible a respuestas ad-hoc `{ "message": "..." }` que varían por endpoint.

## Q59
**Category:** Diseño de API
**Difficulty:** Medium
**Question:** ¿Qué es el versionado de API, y cómo lo implementaría en una aplicación ASP.NET Core?
**Ideal Answer:** El versionado de API permite evolucionar una API sin romper clientes existentes. Estrategias comunes: ruta URL (`/api/v1/users`), cadena de consulta (`?api-version=1.0`), o encabezado (`Api-Version: 1.0`). Use el paquete NuGet `Asp.Versioning.Http`: llame a `builder.Services.AddApiVersioning(options => options.DefaultApiVersion = new ApiVersion(1, 0))`. Anote endpoints o controladores con `[ApiVersion("1.0")]`. Deprecie versiones antiguas con `[ApiVersion("1.0", Deprecated = true)]`. Documente con Swagger usando `Asp.Versioning.Mvc.ApiExplorer`. Prefiera versionado basado en encabezado para APIs que deben mantener URLs limpias; el versionado basado en URL es más visible y más fácil de probar manualmente.

## Q60
**Category:** Diseño de API
**Difficulty:** Hard
**Question:** ¿Qué es HATEOAS, y es práctico implementarlo en APIs REST modernas de .NET?
**Ideal Answer:** HATEOAS (Hipermedia como Motor del Estado de la Aplicación) significa que las respuestas de la API incluyen enlaces a acciones y recursos relacionados, permitiendo a los clientes navegar la API dinámicamente sin codificar URLs de forma fija. Ejemplo: una respuesta `GET /orders/1` incluye `"links": [{"rel": "cancel", "href": "/orders/1/cancel", "method": "POST"}]`. En teoría, desacopla cliente y servidor. En la práctica: la mayoría de APIs del mundo real (incluyendo grandes proveedores cloud) no implementan HATEOAS completo porque los clientes necesitan entender la semántica de los recursos de todos modos, y la sobrecarga de generación de enlaces no está justificada. Rara vez se implementa más allá del Nivel 2 de Madurez de Richardson. Una especificación OpenAPI bien documentada es típicamente más práctica.

## Q61
**Category:** Concurrencia
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre concurrencia optimista y pesimista, y cuándo elegiría cada una en una API .NET?
**Ideal Answer:** Concurrencia optimista: asume que los conflictos son raros. Lee datos, procesa, luego al guardar verifica que la fila no haya cambiado (mediante `rowversion`/`xmin`). Si cambió, lanza `DbUpdateConcurrencyException` y deja que el cliente reintente. Baja sobrecarga, sin bloqueos mantenidos. Concurrencia pesimista: bloquea la fila al leer para que ninguna otra transacción pueda modificarla (`SELECT FOR UPDATE` en Postgres). Garantiza que no haya conflicto pero mantiene bloqueos, reduciendo el rendimiento y arriesgando deadlocks. Use optimista para la mayoría de escenarios de API web (baja contención, transacciones cortas). Use pesimista para escenarios de alta contención donde el costo de resolución de conflictos es alto (por ejemplo, reserva de asientos, deducción de inventario).

## Q62
**Category:** Concurrencia
**Difficulty:** Hard
**Question:** ¿Cómo difiere `SemaphoreSlim` de `lock` para código async, y cuándo usaría cada uno?
**Ideal Answer:** `lock` es sincrónico — bloquea el hilo mientras espera adquirir el monitor. Usar `lock` en código async es peligroso porque no se puede hacer await dentro de un bloque lock, y el bloqueo puede causar agotamiento del ThreadPool. `SemaphoreSlim` soporta espera async: `await semaphore.WaitAsync()` libera el hilo mientras espera, permitiendo que otro trabajo proceda. Use `lock` para secciones críticas sincrónicas muy cortas (por ejemplo, actualizar una colección en memoria). Use `SemaphoreSlim(1, 1)` como mutex async para proteger estado compartido en código async. Use `SemaphoreSlim(N, N)` para limitar el acceso concurrente a un recurso (por ejemplo, máximo 5 llamadas HTTP concurrentes a una API externa).

## Q63
**Category:** Concurrencia
**Difficulty:** Medium
**Question:** ¿Qué es `CancellationToken`, y cómo debería propagarse en una API .NET?
**Ideal Answer:** `CancellationToken` señala que una operación debe cancelarse (por ejemplo, el cliente se desconecta, timeout de solicitud). En ASP.NET Core, el framework inyecta un `CancellationToken` en los parámetros del endpoint/acción automáticamente — se cancela cuando el cliente se desconecta. Propáguelo a través de cada llamada async: consultas a la base de datos (`QueryAsync(..., cancellationToken: ct)`), llamadas de cliente HTTP, I/O de archivos y trabajo en segundo plano. Nunca lo ignore. Cree tokens enlazados para agregar un timeout: `using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct); cts.CancelAfter(TimeSpan.FromSeconds(5))`. La propagación adecuada previene fugas de recursos y ejecución de consultas de base de datos después de que un cliente se ha desconectado.

## Q64
**Category:** Inyección de Dependencias
**Difficulty:** Hard
**Question:** ¿Qué es el antipatrón Service Locator, y cómo difiere del uso legítimo de `IServiceProvider`?
**Ideal Answer:** El patrón Service Locator significa llamar a `serviceProvider.GetService<T>()` desde dentro de la lógica de negocio para resolver dependencias en tiempo de ejecución en lugar de declararlas como parámetros del constructor. Es un antipatrón porque: las dependencias están ocultas (sin contrato explícito de constructor), la clase es más difícil de probar (debe configurar el contenedor), y acopla la lógica de negocio a la infraestructura de DI. Usos legítimos de `IServiceProvider`: crear servicios scoped dentro de un singleton (por ejemplo, `provider.CreateScope()` en un `BackgroundService`), métodos factory en código de infraestructura, y middleware que necesita resolver un servicio scoped de solicitud. En código de dominio y aplicación, siempre use inyección por constructor.

## Q65
**Category:** Inyección de Dependencias
**Difficulty:** Medium
**Question:** ¿Cuáles son las limitaciones de la inyección por constructor en DI de .NET, y cómo maneja dependencias opcionales o condicionales?
**Ideal Answer:** La inyección por constructor requiere que todas las dependencias estén registradas y disponibles. Para dependencias opcionales: use `IServiceProvider.GetService<T>()` (devuelve null si no está registrado) en lugar de `GetRequiredService<T>()`. Alternativamente, declare el parámetro como nullable y use `null` como valor predeterminado. Para dependencias condicionales (diferentes implementaciones por condición en tiempo de ejecución): use un patrón factory — inyecte `IServiceProvider` o un factory `Func<T>`. Para genéricos abiertos (por ejemplo, `IRepository<T>`), registre con `services.AddScoped(typeof(IRepository<>), typeof(Repository<>))`. Evite la sobre-ingeniería: la mayoría de servicios deberían tener una sola implementación concreta.

## Q66
**Category:** Runtime de .NET
**Difficulty:** Hard
**Question:** ¿Cómo funciona el recolector de basura de .NET? ¿Qué son Gen 0, Gen 1 y Gen 2, y qué es el Montón de Objetos Grandes (Large Object Heap)?
**Ideal Answer:** .NET usa un GC generacional. Gen 0: objetos recién asignados de vida corta. Se recolecta frecuentemente, rápido (milisegundos). Gen 1: objetos que sobrevivieron a Gen 0. Gen 2: objetos de larga vida (singletons, cachés). Se recolecta infrecuentemente, puede pausar la aplicación. Montón de Objetos Grandes (LOH): objetos >= 85,000 bytes se asignan aquí y solo se recolectan con Gen 2. LOH no se compacta por defecto, causando fragmentación. Para minimizar la presión del GC: prefiera `Span<T>` y `stackalloc` para búfers temporales, use `ArrayPool<byte>.Shared` para arrays grandes, evite concatenaciones frecuentes de cadenas grandes (use `StringBuilder`), y evite mantener colecciones grandes en Gen 2 si es posible.

## Q67
**Category:** Runtime de .NET
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre `IDisposable` e `IAsyncDisposable`? ¿Cuándo debería implementar cada uno?
**Ideal Answer:** `IDisposable.Dispose()` libera recursos no administrados de forma sincrónica. `IAsyncDisposable.DisposeAsync()` libera recursos de forma asincrónica (por ejemplo, vaciar un stream async, cerrar una conexión de red gracefully). Implemente `IDisposable` cuando su clase mantenga referencias a recursos como handles de archivo, memoria no administrada o conexiones a BD. Implemente `IAsyncDisposable` cuando la limpieza involucre I/O. Implemente ambos si la clase puede ser dispuesta en cualquier contexto. Use `await using` para `IAsyncDisposable`. Nunca llame a `Dispose()` en un objeto aún rastreado por el contenedor DI — deje que el contenedor gestione el ciclo de vida. El contenedor DI de ASP.NET Core llama a `DisposeAsync()` para servicios scoped/transient que lo implementan.

## Q68
**Category:** Runtime de .NET
**Difficulty:** Hard
**Question:** ¿Qué es Native AOT en .NET, y qué restricciones impone en el código de la aplicación?
**Ideal Answer:** Native AOT (compilación Ahead-of-Time) compila código .NET a un binario nativo antes del despliegue, eliminando el compilador JIT en tiempo de ejecución. Beneficios: tiempo de inicio más rápido (milisegundos vs. cientos de milisegundos), menor huella de memoria, despliegue más pequeño (sin runtime necesario), mejor arranque en frío de contenedores. Restricciones: sin generación de código en tiempo de ejecución (sin `Reflection.Emit`, sin árboles de expresión compilados en runtime), reflexión limitada (debe usar generadores de código fuente en su lugar), sin carga dinámica de ensamblados, sin patrones `Type.GetType(string)`. Las minimal APIs de ASP.NET Core y `System.Text.Json` con generadores de código fuente están diseñados para compatibilidad con AOT. EF Core tiene soporte limitado de AOT. Mejor suited para funciones Lambda, microservicios y herramientas CLI.

## Q69
**Category:** Runtime de .NET
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre internado de `string`, `StringBuilder` y `string.Create` en términos de asignación de memoria?
**Ideal Answer:** `string` es inmutable — cada concatenación crea un nuevo objeto string. El internado de strings agrupa literales idénticos para reutilizar la misma referencia (`string.Intern`), útil para reducir asignaciones con valores repetidos pero riesgoso para fugas de memoria si se sobreusa. `StringBuilder` acumula caracteres en un búfer mutable y solo asigna la cadena final en `ToString()` — ideal para construir cadenas en bucles. `string.Create(length, state, spanAction)` es lo más eficiente en asignación: asigna exactamente una cadena y escribe en ella mediante un `Span<char>`, evitando copias intermedias. Use `StringBuilder` para construcción de longitud variable; `string.Create` para rutas críticas de rendimiento de longitud fija.

## Q70
**Category:** Cloud Native
**Difficulty:** Hard
**Question:** ¿Qué es la metodología de la Aplicación de 12 Factores, y cómo aplican los factores más relevantes a un microservicio .NET?
**Ideal Answer:** 12-Factor define mejores prácticas para aplicaciones cloud-native. Los más relevantes para .NET: (1) Configuración: almacene la configuración en variables de entorno, nunca en código. Use `IConfiguration` + `AddEnvironmentVariables()`. (2) Servicios de respaldo: trate BD, cola, caché como recursos adjuntos mediante configuración, no codificados de forma fija. (3) Procesos: sea stateless — sin sesiones en memoria, sin sesiones sticky. (4) Logs: trate los logs como flujos de eventos — escriba a stdout/stderr, no a archivos. Use logging estructurado. (5) Desechabilidad: inicio rápido (< 5s), apagado graceful en SIGTERM. Implemente `IHostedService.StopAsync`. (6) Paridad dev/prod: use Docker Compose localmente para igualar producción. (7) Dependencias: declare todas las dependencias en `.csproj`, sin dependencias implícitas a nivel de sistema.

## Q71
**Category:** Cloud Native
**Difficulty:** Medium
**Question:** ¿Qué es la verificación de salud en ASP.NET Core, y cómo configuraría sondas de vivacidad vs. disponibilidad para Kubernetes?
**Ideal Answer:** ASP.NET Core proporciona `AddHealthChecks()` y `MapHealthChecks()`. Sonda de vivacidad: verifica si el proceso está vivo y no en deadlock. Debe ser muy ligera — solo devolver Healthy. Mapee a `/health/live` y verifique solo a sí mismo: `.AddCheck("self", () => HealthCheckResult.Healthy())`. Sonda de disponibilidad: verifica si la aplicación está lista para recibir tráfico (BD conectada, dependencias activas). Mapee a `/health/ready` e incluya todas las verificaciones: `.AddDbContextCheck<AppDbContext>()`. En Kubernetes, configure `livenessProbe` en `/health/live` y `readinessProbe` en `/health/ready`. Sepárelas para evitar matar un pod que solo está temporalmente incapaz de alcanzar la BD.

## Q72
**Category:** Cloud Native
**Difficulty:** Hard
**Question:** ¿Cómo implementaría circuit breaking y políticas de reintento en una aplicación .NET que llama a APIs HTTP externas?
**Ideal Answer:** Use Polly (o `Microsoft.Extensions.Http.Resilience` en .NET 8+). Política de reintento: reintente en errores transitorios (5xx, timeouts de red) con backoff exponencial + jitter para evitar efecto manada. Circuit breaker: después de N fallos consecutivos, abra el circuito por una duración y falle rápidamente sin llamar al servicio — previene fallos en cascada y permite la recuperación. Configure mediante `services.AddHttpClient<MyClient>().AddResilienceHandler(...)` o `AddTransientHttpErrorPolicy`. Consideraciones clave: no reintente operaciones no idempotentes (POST) a menos que el endpoint sea idempotente. Registre transiciones de estado del circuito. Combine con políticas de timeout. En sistemas distribuidos, la resiliencia no es opcional.

## Q73
**Category:** Cloud Native
**Difficulty:** Medium
**Question:** ¿Qué es la containerización, y cuáles son las mejores prácticas para escribir un Dockerfile para una API .NET?
**Ideal Answer:** Mejores prácticas para Dockerfiles .NET: (1) Use compilaciones multi-etapa — imagen SDK para compilación, imagen runtime para la final (reduce el tamaño de imagen de ~700MB a ~200MB). (2) Restaure paquetes antes de copiar el código fuente para que Docker cachee la capa de restauración. (3) Use la imagen `mcr.microsoft.com/dotnet/aspnet` (no SDK) para la etapa de runtime. (4) Ejecute como usuario no-root por seguridad. (5) Establezca `ASPNETCORE_URLS=http://+:8080` para evitar ejecutar en el puerto 80. (6) Use `.dockerignore` para excluir `bin/`, `obj/`, `.git/`. (7) Para aplicaciones compiladas con AOT, use la imagen base `chiseled` de Ubuntu para superficie de ataque mínima.

## Q74
**Category:** Habilidades Blandas y Proceso
**Difficulty:** Medium
**Question:** ¿Cómo aborda la revisión de un pull request como desarrollador senior? ¿Qué busca más allá de la corrección?
**Ideal Answer:** Más allá de la corrección: (1) Diseño: ¿el cambio introduce complejidad innecesaria? ¿Podría ser más simple? ¿Es consistente con la arquitectura existente? (2) Seguridad: ¿algún riesgo de inyección, verificaciones de autorización faltantes, secretos en logs? (3) Rendimiento: ¿consultas N+1, colecciones sin límite, índices faltantes, sincrónico sobre async? (4) Testabilidad: ¿los cambios están probados? ¿Las pruebas prueban comportamiento o detalles de implementación? (5) Manejo de errores: ¿los errores se manejan gracefully o se tragan silenciosamente? (6) Observabilidad: ¿los eventos importantes se registran con contexto? (7) Nomenclatura y legibilidad: ¿un recién llegado entendería esto en 6 meses? (8) Cambios incompatibles: ¿esto afecta a clientes o contratos existentes?

## Q75
**Category:** Habilidades Blandas y Proceso
**Difficulty:** Medium
**Question:** ¿Cómo maneja la deuda técnica en un producto que se mueve rápidamente? ¿Cómo decide cuándo refactorizar vs. entregar?
**Ideal Answer:** La deuda técnica es inevitable; la clave es gestionarla intencionalmente. Estrategias: (1) Registre la deuda como elementos explícitos del backlog, no como conocimiento invisible. (2) Aplique la Regla del Boy Scout: deje el código ligeramente mejor de como lo encontró. (3) Refactorice en el contexto de una funcionalidad relacionada — no cree PRs de limpieza no relacionados que detengan la revisión. (4) Use el patrón Strangler Fig para refactorizaciones grandes: construya el nuevo comportamiento junto al viejo y migre incrementalmente. (5) Distinga entre deuda deliberada (atajo consciente con un plan) y deuda imprudente (sin plan). Entregue cuando la deuda no bloquee el objetivo actual y esté registrada. Refactorice cuando la deuda esté activamente ralentizando al equipo o introduciendo errores.

## Q76
**Category:** Habilidades Blandas y Proceso
**Difficulty:** Hard
**Question:** ¿Cómo abordaría la migración de un monolito legacy de .NET Framework a .NET 8+ sin una reescritura completa?
**Ideal Answer:** Patrón Strangler Fig: reemplace incrementalmente partes del monolito en lugar de reescribir todo de una vez. Pasos: (1) Identifique una porción delimitada (por ejemplo, un subsistema individual) que pueda extraerse con dependencias mínimas. (2) Cree un nuevo proyecto ASP.NET Core junto al monolito. (3) Use un API Gateway o reverse proxy (YARP, NGINX) para enrutar el tráfico de esa porción al nuevo servicio. (4) Comparta la base de datos inicialmente (antipatrón pero pragmático); extraiga a esquema/BD separado con el tiempo. (5) Migre las bibliotecas compartidas a `netstandard2.0` primero para compatibilidad, luego a `net8.0`. Evite reescribir lógica de negocio desde cero — pórtela, mantenga las pruebas en verde.

## Q77
**Category:** Diseño de API
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre REST y gRPC? ¿Cuándo elegiría gRPC para un servicio .NET?
**Ideal Answer:** REST usa HTTP/1.1 con JSON, es legible por humanos y es el estándar para APIs públicas. gRPC usa HTTP/2 con Protocol Buffers (binario), proporcionando: menor tamaño de payload, streaming bidireccional, generación de código desde contratos `.proto`, y contratos de API estrictos. En .NET, use `Grpc.AspNetCore`. Elija gRPC para: comunicación interna entre servicios de alto rendimiento, microservicios políglotas que necesitan contratos fuertemente tipados, streaming bidireccional (por ejemplo, telemetría en tiempo real), y clientes móviles donde el ancho de banda importa. Elija REST para: APIs públicas, clientes de navegador (gRPC-Web agrega complejidad), o cuando la legibilidad humana y las herramientas (Postman, Swagger) son prioritarias.

## Q78
**Category:** Diseño de API
**Difficulty:** Hard
**Question:** ¿Cómo diseñaría una API de paginación? Compare la paginación basada en offset vs. basada en cursor y sus ventajas y desventajas.
**Ideal Answer:** Basada en offset (`?page=2&pageSize=20`): simple de implementar y soporta saltar a páginas arbitrarias. Desventaja: resultados inconsistentes cuando se insertan/eliminan datos durante la paginación (las filas se desplazan), y `OFFSET N` en SQL se vuelve lento para N grandes (la BD escanea todas las filas precedentes). Basada en cursor: la respuesta incluye un cursor (token opaco que codifica el último ID visto o clave de ordenamiento). La siguiente solicitud pasa `?cursor=...` para obtener filas después de ese cursor. Beneficios: resultados consistentes independientemente de escrituras concurrentes, tiempo de búsqueda O(1) con el índice correcto. Desventaja: no puede saltar a páginas arbitrarias, más difícil de implementar. Use offset para grillas administrativas con datos moderados; use cursor para feeds, flujos de eventos y tablas grandes.

## Q79
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Qué es Event Sourcing, y cuáles son las ventajas y desventajas comparado con un modelo de persistencia tradicional basado en estado?
**Ideal Answer:** Event Sourcing persiste la secuencia de eventos de dominio que llevaron al estado actual, en lugar del estado actual en sí. Se reconstruye el estado reproduciendo eventos. Beneficios: registro de auditoría completo, consultas de viaje en el tiempo, capacidad de proyectar estado en múltiples modelos de lectura, ajuste natural para CQRS. Desventajas: las consultas requieren proyecciones (modelos de lectura) — no se puede simplemente hacer `SELECT * FROM orders`. La evolución del esquema de eventos es compleja. Consistencia eventual entre modelos de escritura y lectura. Reconstruir el estado a partir de muchos eventos puede ser lento sin snapshots. Alta complejidad operacional. Mejor suited para dominios con requisitos inherentes de auditoría (finanzas, cumplimiento) o donde el historial de eventos es una funcionalidad de primera clase. Excesivo para aplicaciones CRUD simples.

## Q80
**Category:** Arquitectura
**Difficulty:** Medium
**Question:** ¿Qué es el patrón de Capa Anti-Corrupción (ACL) en DDD, y cuándo es necesario?
**Ideal Answer:** Un ACL es una capa de traducción entre dos contextos delimitados o sistemas con modelos diferentes. Al integrar con un sistema legacy, API externa o servicio de terceros, los conceptos del modelo externo no deberían filtrarse a su modelo de dominio. El ACL traduce DTOs, eventos o estructuras externas al lenguaje de su dominio. Ejemplo: una pasarela de pago externa devuelve valores de `TransactionStatus` en su propio formato; el ACL los mapea al enum `PaymentStatus` de su dominio. Sin un ACL, los cambios en el modelo externo fuerzan cambios en todo su dominio. Impleméntelo como adaptadores, mapeadores o un servicio de traducción dedicado. Siempre use un ACL cuando el modelo externo está fuera de su control.

## Q81
**Category:** Azure
**Difficulty:** Hard
**Question:** ¿Qué es Azure Event Grid, y cómo difiere de Azure Service Bus y Azure Event Hubs?
**Ideal Answer:** Event Grid: enrutamiento reactivo de eventos para eventos de recursos de Azure y eventos personalizados. Baja latencia, push HTTP (webhooks), serverless. Mejor para reaccionar a cambios de recursos de Azure (Blob creado, grupo de recursos eliminado) o eventos personalizados ligeros. Sin ordenamiento ni reproducción. Service Bus: broker de mensajes empresarial. Procesamiento ordenado, cola de mensajes no entregados, transacciones, sesiones. Mejor para procesamiento confiable y ordenado de comandos entre servicios. Event Hubs: streaming de eventos de alto rendimiento (millones de eventos/seg). Retiene eventos por 24h–90 días, soporta grupos de consumidores para procesamiento paralelo, se integra con el protocolo Apache Kafka. Mejor para telemetría, agregación de logs, pipelines de analítica en tiempo real. La elección depende del volumen, necesidades de ordenamiento y modelo de consumidor.

## Q82
**Category:** Azure
**Difficulty:** Medium
**Question:** ¿Qué es Azure Redis Cache, y cómo implementaría una caché distribuida en una API ASP.NET Core?
**Ideal Answer:** Azure Cache for Redis es una instancia Redis administrada. Intégrela con ASP.NET Core mediante `services.AddStackExchangeRedisCache(options => options.Configuration = config["Redis:ConnectionString"])`. Use `IDistributedCache.GetAsync/SetAsync` con serialización de array de bytes, o envuélvalo con un helper tipado usando `System.Text.Json`. Para caché fuertemente tipada, use `StackExchange.Redis` directamente para operaciones más ricas (sets, sorted sets, pub-sub). Patrones comunes: cache-aside (verificar caché, en caso de fallo cargar de BD y poblar caché con expiración), write-through (actualizar BD y caché atómicamente). Establezca TTLs apropiados. Use bloqueo distribuido (`RedLock`) para prevención de estampida de caché en claves de alto tráfico.

## Q83
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre métodos `abstract` y `virtual` en C#? ¿Cuándo usaría una interfaz vs. una clase abstracta?
**Ideal Answer:** `virtual`: proporciona una implementación predeterminada que las clases derivadas pueden sobrescribir. `abstract`: declara un método sin implementación; las clases derivadas deben sobrescribirlo. Una clase abstracta puede tener tanto miembros abstractos como concretos, no puede instanciarse y puede mantener estado. Interfaz: contrato puro sin estado (campos), soporta herencia múltiple, y desde C# 8 puede tener implementaciones predeterminadas. Use una clase abstracta cuando: comparta implementación a través de una jerarquía, necesite una base común con implementación parcial, o necesite miembros protegidos. Use una interfaz cuando: defina un contrato que múltiples tipos no relacionados implementarán, o habilite inyección de dependencias con múltiples implementaciones.

## Q84
**Category:** Lenguaje C#
**Difficulty:** Hard
**Question:** ¿Qué son los árboles de expresión en C#, y cómo los usa EF Core para traducir LINQ a SQL?
**Ideal Answer:** Un árbol de expresión representa código como datos — un árbol de nodos `Expression` que puede inspeccionarse y transformarse en tiempo de ejecución. Cuando escribe `context.Users.Where(u => u.Email == email)`, la lambda se captura como `Expression<Func<User, bool>>` (no compilada a un delegado). El proveedor LINQ de EF Core recorre este árbol de expresión y lo traduce a SQL: `WHERE email = @email`. Por eso EF Core puede traducir LINQ a SQL pero no puede traducir llamadas a métodos C# arbitrarios (los métodos no comprendidos por el proveedor lanzan `InvalidOperationException`). Los árboles de expresión también se usan en mapeo ORM, constructores de consultas dinámicas (patrón Specification) y generadores de código fuente.

## Q85
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** ¿Qué son los tipos de referencia nullable en C# 8+, y cómo ayudan a prevenir `NullReferenceException`?
**Ideal Answer:** Los tipos de referencia nullable (NRT) introducen análisis de nulabilidad en tiempo de compilación. Cuando se habilitan (`<Nullable>enable</Nullable>`), los tipos de referencia son no-nullable por defecto — `string name` garantiza no-null, `string? name` permite explícitamente null. El compilador emite advertencias cuando: un valor nullable se usa sin verificación de null, o cuando un parámetro/propiedad no-nullable podría recibir null. Esto detecta errores de `NullReferenceException` en tiempo de compilación en lugar de en tiempo de ejecución. Migre código habilitando NRT por archivo primero, agregando anotaciones `?` y verificaciones de null. NRT no cambia el comportamiento en runtime — es puramente una herramienta de análisis en tiempo de compilación.

## Q86
**Category:** Patrones de Diseño
**Difficulty:** Medium
**Question:** ¿Qué es el patrón Observer, y cómo se implementa en .NET? Dé un ejemplo más allá de los delegados `event`.
**Ideal Answer:** El patrón Observer define una dependencia de uno-a-muchos donde los observadores son notificados de cambios de estado. En .NET: (1) Delegados `event` — la forma más simple; los publicadores disparan eventos y los suscriptores registran manejadores. (2) `IObservable<T>`/`IObserver<T>` — el modelo reactivo basado en push usado por Reactive Extensions (Rx.NET). (3) `INotifyPropertyChanged` — usado en enlace de datos de WPF/MAUI. (4) Notificaciones de MediatR — `INotificationHandler<T>` para despacho de eventos de dominio en proceso. (5) Canales (`System.Threading.Channels`) — una cola productor/consumidor para comunicación desacoplada dentro de un proceso. Elija según si necesita notificación sincrónica/async, contrapresión o distribución fan-out.

## Q87
**Category:** Rendimiento
**Difficulty:** Hard
**Question:** ¿Qué es `ArrayPool<T>` y `MemoryPool<T>`, y cuándo deberían usarse en lugar de `new T[]`?
**Ideal Answer:** `ArrayPool<T>.Shared` proporciona un pool de arrays reutilizables, evitando la asignación repetida en el heap y la presión del GC para búfers grandes temporales. Alquile con `ArrayPool<byte>.Shared.Rent(minLength)` y devuelva con `ArrayPool<byte>.Shared.Return(buffer, clearArray: true)`. El array devuelto puede ser más grande de lo solicitado — siempre rastree el tamaño real usado. `MemoryPool<T>` es la contraparte abstracta y segura para async que devuelve `IMemoryOwner<T>` que implementa `IDisposable` para devolución segura mediante `using`. Casos de uso: lectura del cuerpo de solicitudes HTTP, análisis de protocolos binarios, I/O de archivos, cualquier código que asigne arrays de bytes de vida corta en rutas frecuentes. Nunca olvide devolver el array alquilado — no hacerlo causa agotamiento del pool.

## Q88
**Category:** Seguridad
**Difficulty:** Hard
**Question:** ¿Qué es PKCE, y por qué es requerido para flujos de código de autorización OAuth 2.0 en clientes nativos/SPA?
**Ideal Answer:** PKCE (Proof Key for Code Exchange) previene ataques de interceptación de código de autorización en clientes públicos (aplicaciones nativas, SPAs) que no pueden almacenar de forma segura un secreto de cliente. Flujo: el cliente genera un `code_verifier` aleatorio, calcula `code_challenge = BASE64URL(SHA256(code_verifier))`, e incluye el challenge en la solicitud de autorización. Al intercambiar el código por un token, el cliente envía el `code_verifier` original. El servidor verifica que coincida con el challenge. Un atacante que intercepte el código de autorización no puede intercambiarlo sin el `code_verifier` original. PKCE es obligatorio para clientes públicos según OAuth 2.1 y recomendado para todos los flujos de código de autorización independientemente del tipo de cliente.

## Q89
**Category:** Pruebas
**Difficulty:** Hard
**Question:** ¿Qué son las pruebas de mutación, y cómo complementan las métricas de cobertura de código?
**Ideal Answer:** Las pruebas de mutación evalúan la calidad de la suite de pruebas introduciendo pequeños errores intencionales (mutaciones) en el código (por ejemplo, cambiando `>` a `>=`, invirtiendo un booleano) y verificando que las pruebas existentes los detecten (maten al mutante). Si una mutación sobrevive (las pruebas siguen pasando con un error en el código), la suite de pruebas tiene una brecha. La cobertura de código solo mide si una línea fue ejecutada, no si la prueba afirma comportamiento significativo. Una prueba puede lograr 100% de cobertura sin afirmar nada. Las pruebas de mutación proporcionan una señal mucho más alta. Herramientas: Stryker.NET para C#. Puntuaciones altas de mutación indican que las pruebas están genuinamente verificando comportamiento, no solo ejercitando rutas de código.

## Q90
**Category:** Bases de Datos
**Difficulty:** Hard
**Question:** ¿Cuál es una estrategia de migración de base de datos para despliegues sin tiempo de inactividad? ¿Cómo maneja cambios de esquema incompatibles?
**Ideal Answer:** Las migraciones sin tiempo de inactividad requieren compatibilidad hacia atrás entre las versiones de código antigua y nueva. Estrategia: (1) Expandir: agregue nueva columna como nullable (sin default requerido, el código viejo la ignora, el código nuevo escribe en ella). (2) Migrar: rellene datos en lotes para evitar bloqueos de tabla. (3) Contraer: después de que todas las instancias ejecuten el código nuevo, agregue restricciones o elimine columnas antiguas. Nunca agregue una columna NOT NULL sin default en un solo despliegue. Nunca renombre columnas o tablas en un paso — agregue nueva, migre, elimine antigua en múltiples despliegues. Use feature flags para desacoplar el despliegue de la activación de funcionalidades. Las migraciones de EF Core deben aplicarse separadamente del despliegue de código (por ejemplo, como un init container en Kubernetes).

## Q91
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Qué es el patrón Saga para transacciones distribuidas? Compare coreografía vs. orquestación.
**Ideal Answer:** Una Saga es una secuencia de transacciones locales coordinadas para lograr una transacción distribuida sin un commit de dos fases. Cada paso tiene una transacción compensatoria para reversión. Coreografía: cada servicio escucha eventos y reacciona, emitiendo más eventos. Descentralizado, sin punto único de fallo, pero difícil de visualizar y depurar el flujo general. Orquestación: un coordinador central (orquestador de saga) llama a cada servicio en secuencia y emite comandos compensatorios en caso de fallo. Más fácil de entender y monitorear, pero el orquestador puede convertirse en un cuello de botella. En .NET, las Sagas de MassTransit implementan orquestación como máquinas de estados. Use sagas para: cumplimiento de pedidos, flujos de pago multi-paso, y cualquier proceso que abarque múltiples servicios.

## Q92
**Category:** Bases de Datos
**Difficulty:** Medium
**Question:** ¿Cuál es la diferencia entre Dapper y EF Core, y cuándo usaría cada uno en el mismo proyecto?
**Ideal Answer:** EF Core: ORM completo con seguimiento de cambios, migraciones, traducción de LINQ a SQL, gestión de relaciones. Ideal para operaciones de escritura (comandos) donde necesita seguimiento de cambios, control de concurrencia y gestión de esquemas. Dapper: micro-ORM — envoltura delgada sobre ADO.NET que mapea resultados de consultas SQL a objetos. Sin seguimiento de cambios, sin traducción LINQ. Más rápido y más simple para operaciones de lectura. Patrón ideal en CQRS: los manejadores de comandos usan EF Core para escrituras (aprovechando seguimiento de cambios y migraciones), los manejadores de consultas usan Dapper para lecturas (SQL optimizado, proyecciones, JOINs, agregaciones). Comparta la misma conexión `IDbConnection`/`DbContext`. Esto le da la corrección de EF Core para escrituras y el rendimiento de SQL crudo para lecturas.

## Q93
**Category:** Lenguaje C#
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre `Task.WhenAll` y `Task.WhenAny`? ¿Cuáles son los riesgos de usar `Task.WhenAll` sin manejo de errores adecuado?
**Ideal Answer:** `Task.WhenAll` se completa cuando todas las tareas terminan — si alguna tarea falla, relanza la primera excepción; otras excepciones se almacenan en `AggregateException.InnerExceptions`. `Task.WhenAny` se completa tan pronto como la primera tarea se completa (o falla). Riesgo con `WhenAll`: si hace `await` sin un try/catch, solo la primera excepción se muestra; las demás se ignoran silenciosamente, potencialmente ocultando fallos. Para inspeccionar todas las excepciones: capture `AggregateException` o verifique cada tarea individualmente después de `WhenAll`. Otro riesgo: si las tareas no están limitadas, lanzar miles de tareas simultáneamente puede agotar el ThreadPool o el pool de conexiones. Use `SemaphoreSlim` para limitar la concurrencia al procesar colecciones grandes.

## Q94
**Category:** Azure
**Difficulty:** Medium
**Question:** ¿Qué es Azure Cosmos DB, y qué ventajas y desventajas tiene comparado con Azure SQL Database para un backend .NET?
**Ideal Answer:** Cosmos DB es una base de datos NoSQL distribuida globalmente, multi-modelo con latencia garantizada de milisegundos de un solo dígito y niveles de consistencia configurables (fuerte a eventual). Fortalezas: escalado horizontal, geo-replicación, flexibilidad de esquema, múltiples APIs (SQL, Mongo, Cassandra). Desventajas vs. Azure SQL: sin transacciones ACID entre particiones (limitado a partición única o multi-documento dentro de una partición en versiones más recientes), sin JOINs arbitrarios, el diseño del esquema debe alinearse con patrones de acceso, y el modelo de facturación por RU (Unidad de Solicitud) puede ser costoso y difícil de predecir. Use Cosmos DB para: cargas de trabajo de alto volumen, distribuidas globalmente con patrones de acceso bien definidos. Use Azure SQL para: datos relacionales, consultas complejas, reportes, y cuando las garantías ACID son críticas.

## Q95
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Qué es el patrón Strangler Fig, y cómo lo aplicaría para descomponer un monolito incrementalmente?
**Ideal Answer:** Nombrado así por un árbol que lentamente rodea a su huésped, el patrón Strangler Fig reemplaza incrementalmente un sistema legacy construyendo nueva funcionalidad en un nuevo sistema junto al viejo, redirigiendo tráfico porción por porción. Pasos: (1) Identifique una porción (una funcionalidad o límite de dominio) para extraer. (2) Construya el nuevo servicio implementando esa porción. (3) Use una fachada o reverse proxy (YARP, NGINX, API Gateway) para enrutar el tráfico de esa porción al nuevo servicio. (4) Verifique la corrección, luego descomisione el código viejo. (5) Repita. Beneficios: sin reescritura masiva, entrega continua, reversible en cada paso. Desafío clave: compartir base de datos durante la transición — use BD compartida inicialmente, extraiga la propiedad del esquema con el tiempo con capas de traducción ACL.

## Q96
**Category:** Lenguaje C#
**Difficulty:** Medium
**Question:** ¿Qué son los setters solo `init` en C# 9, y cómo soportan la construcción de objetos inmutables?
**Ideal Answer:** Los setters `init` solo pueden llamarse durante la inicialización del objeto (en el constructor o un inicializador de objeto), no después. Permiten: `var obj = new MyClass { Name = "test" }` (sintaxis legible) mientras previenen la mutación después de la construcción (`obj.Name = "other"` es un error de compilación). A diferencia de los campos `readonly`, las propiedades `init` pueden usarse en inicializadores de objeto y expresiones `with` para records. Son ideales para DTOs y objetos de valor que deben ser inmutables post-construcción mientras soportan sintaxis de inicialización legible. Combinados con el modificador `required` (C# 11), imponen que la propiedad se establezca en el momento de la inicialización.

## Q97
**Category:** Patrones de Diseño
**Difficulty:** Hard
**Question:** ¿Qué es el patrón Strategy, y cómo se compara con el uso de polimorfismo (herencia) en .NET?
**Ideal Answer:** El patrón Strategy define una familia de algoritmos, encapsula cada uno y los hace intercambiables en tiempo de ejecución mediante composición. En .NET: inyecte `IPaymentStrategy` en un manejador; en tiempo de ejecución inyecte `StripePaymentStrategy` o `PayPalPaymentStrategy`. Con herencia (polimorfismo), el algoritmo está integrado en la jerarquía de clases — cambiar el comportamiento requiere una subclase diferente, y múltiples comportamientos requieren múltiples niveles de herencia. Strategy usa composición: el comportamiento es un objeto separado e inyectable. Beneficios: más fácil agregar nuevas estrategias sin modificar clases existentes (abierto/cerrado), más fácil de probar cada estrategia de forma aislada, y soporta selección en tiempo de ejecución. Prefiera composición sobre herencia para variación de comportamiento.

## Q98
**Category:** Runtime de .NET
**Difficulty:** Hard
**Question:** ¿Cuál es la diferencia entre `Thread`, `Task`, `ThreadPool` y `async/await` en la concurrencia de .NET?
**Ideal Answer:** `Thread`: un hilo a nivel del SO. Costoso (1MB de pila por defecto). Use solo para trabajo de bloqueo de larga duración. `ThreadPool`: un pool de hilos reutilizables gestionado por el CLR. Las tareas se encolan en él. Expandir el pool es lento. `Task`: representa una operación asincrónica — puede ejecutarse en un hilo del ThreadPool o completarse sin un hilo (I/O puro). `async/await`: azúcar sintáctico sobre `Task` que genera una máquina de estados; el método se suspende en cada `await`, liberando el hilo para otro trabajo, y se reanuda cuando la operación esperada se completa. Para trabajo limitado por I/O (base de datos, HTTP), `async/await` es ideal — ningún hilo está bloqueado mientras espera. Para trabajo limitado por CPU, use `Task.Run` para descargar al ThreadPool sin bloquear el hilo de la solicitud.

## Q99
**Category:** Diseño de API
**Difficulty:** Medium
**Question:** ¿Qué es FluentValidation, y cómo lo integra con una API ASP.NET Core para validación de solicitudes?
**Ideal Answer:** FluentValidation proporciona una API fluida para construir reglas de validación en clases de validador dedicadas, separando la lógica de validación del dominio y el código del controlador. Defina `class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>` con reglas como `RuleFor(x => x.Email).NotEmpty().EmailAddress()`. Registre con `services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>()`. En minimal APIs o MediatR, valide manualmente: `var result = validator.Validate(command); if (!result.IsValid) return Results.ValidationProblem(result.ToDictionary())`. Para controladores, use la integración `FluentValidation.AspNetCore` para engancharse en `ModelState` automáticamente. Comparado con Data Annotations, FluentValidation es más expresivo, soporta reglas condicionales, validación entre propiedades y es más fácil de probar unitariamente.

## Q100
**Category:** Arquitectura
**Difficulty:** Hard
**Question:** ¿Cómo diseña para la observabilidad desde el día uno en un microservicio .NET? ¿Cuál es la diferencia entre monitoreo y observabilidad?
**Ideal Answer:** El monitoreo responde "¿está el sistema activo?" con métricas y alertas predefinidas. La observabilidad responde "¿por qué el sistema se comporta de esta manera?" explorando modos de fallo desconocidos a través de telemetría (logs, métricas, trazas). Diseñe para la observabilidad: (1) Logging estructurado con IDs de correlación y contexto de solicitud (Serilog + enrichers). (2) Rastreo distribuido con OpenTelemetry — auto-instrumente HTTP, BD y mensajería; propague contexto de traza. (3) Métricas: tasa de solicitudes, tasa de errores, percentiles de latencia (p50, p95, p99), profundidad de cola — exponga mediante endpoint Prometheus u OTLP. (4) Verificaciones de salud con verificaciones significativas (no solo "vivo"). (5) Alertas sobre violaciones de SLO, no solo sobre la salud del servidor. La observabilidad es una preocupación de ingeniería de primera clase, no algo que se agrega después.
