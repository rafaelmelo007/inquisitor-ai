# Desenvolvedor Sênior .NET Backend

## Q1
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** Qual é a diferença entre `ValueTask<T>` e `Task<T>`, e quando você deve preferir um em relação ao outro?
**Ideal Answer:** `Task<T>` sempre aloca um objeto no heap. `ValueTask<T>` é uma struct que evita alocação quando o resultado já está disponível de forma síncrona (por exemplo, valores em cache, caminhos frequentes). Prefira `ValueTask<T>` para APIs de alto throughput onde a operação frequentemente completa de forma síncrona. Evite-o quando o resultado é sempre assíncrono, pois aguardar repetidamente ou armazená-lo é inseguro e pode levar a comportamento indefinido. `Task<T>` é mais seguro para uso geral; `ValueTask<T>` é uma otimização.

## Q2
**Category:** Linguagem C#
**Difficulty:** Hard
**Question:** Explique como `IAsyncEnumerable<T>` funciona e descreva um caso de uso real onde é preferível retornar um `List<T>` ou `IEnumerable<T>`.
**Ideal Answer:** `IAsyncEnumerable<T>` permite iteração assíncrona usando `await foreach`. Os itens são produzidos um de cada vez conforme ficam disponíveis, sem armazenar a coleção inteira em buffer. É ideal para streaming de grandes conjuntos de resultados de um banco de dados (por exemplo, lendo milhares de linhas com `QueryUnbufferedAsync` do Dapper), streaming de respostas de API ou leitura de filas de mensagens. Retornar um `List<T>` materializa tudo na memória primeiro; `IAsyncEnumerable<T>` reduz a pressão de memória e o tempo até o primeiro resultado.

## Q3
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** O que são construtores primários no C# 12, e quais são os prós e contras de usá-los em classes vs. records?
**Ideal Answer:** Construtores primários permitem que parâmetros sejam declarados diretamente na declaração da classe ou struct. Em records, eles estão disponíveis desde o C# 9 e geram propriedades públicas somente-inicialização automaticamente. Em classes (C# 12), os parâmetros ficam no escopo de todo o corpo da classe, mas não são automaticamente promovidos a propriedades — eles são capturados como campos privados se referenciados. Isso significa que podem ser mutados a menos que se tome cuidado. Prós e contras: reduzem código boilerplate, mas podem ser menos explícitos sobre se um parâmetro se torna uma propriedade ou um campo, o que pode prejudicar a legibilidade.

## Q4
**Category:** Linguagem C#
**Difficulty:** Hard
**Question:** Qual é a diferença entre `Span<T>`, `Memory<T>` e `ArraySegment<T>`? Quando cada um é apropriado?
**Ideal Answer:** `Span<T>` é uma ref struct somente de pilha que fornece uma janela sobre memória contígua (array, buffer alocado na pilha ou memória nativa) sem alocação no heap. Não pode ser armazenado em campos nem usado através de limites assíncronos. `Memory<T>` é o equivalente compatível com heap do `Span<T>`, seguro para armazenar em campos e usar com código assíncrono. `ArraySegment<T>` é o predecessor mais antigo e menos eficiente — funciona apenas com arrays e tem mais overhead. Use `Span<T>` para parsing síncrono, de alta performance e confinado à pilha; use `Memory<T>` para cenários assíncronos; evite `ArraySegment<T>` em código novo.

## Q5
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** Explique o tipo `record` em C#. O que o compilador gera, e o que é igualdade estrutural vs. referencial?
**Ideal Answer:** Um `record` é um tipo por referência (ou `record struct` para tipo por valor) onde o compilador gera: `Equals` e `GetHashCode` baseados em valor comparando todas as propriedades, um `ToString` que imprime os valores das propriedades, um construtor de cópia e um método de desconstrução. Igualdade estrutural significa que duas instâncias com valores de propriedades idênticos são consideradas iguais, diferente de classes que usam igualdade referencial (mesmo endereço de memória). Records também suportam mutação não-destrutiva via expressões `with`. São ideais para DTOs e modelos de domínio imutáveis.

## Q6
**Category:** Linguagem C#
**Difficulty:** Hard
**Question:** O que são source generators, e como diferem de Roslyn analyzers e templates T4?
**Ideal Answer:** Source generators executam durante a compilação e emitem arquivos-fonte C# adicionais na compilação. Diferente de templates T4 (que executam em tempo de design fora do pipeline do compilador), source generators têm acesso completo ao modelo semântico via APIs do Roslyn e podem inspecionar o código sendo compilado. Diferente de analyzers (que apenas reportam diagnósticos), generators produzem código novo. Casos de uso incluem eliminação de serialização baseada em reflexão (por exemplo, source gen do `System.Text.Json`), geração de código de mapeamento ou produção de acessores de configuração fortemente tipados. Eles melhoram o desempenho de inicialização e a compatibilidade com AOT.

## Q7
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** Qual é a diferença entre covariância e contravariância em genéricos? Dê um exemplo prático.
**Ideal Answer:** Covariância (`out T`) permite que um tipo mais derivado seja usado onde um tipo base é esperado, válido para produtores (por exemplo, `IEnumerable<string>` pode ser atribuído a `IEnumerable<object>`). Contravariância (`in T`) permite um tipo mais geral onde um derivado é esperado, válido para consumidores (por exemplo, `Action<object>` pode ser atribuído a `Action<string>`). Apenas interfaces e delegates suportam variância. Um exemplo prático: `IEnumerable<T>` é covariante, então `IEnumerable<Dog>` é atribuível a `IEnumerable<Animal>` — seguro porque você apenas lê dele e nunca escreve.

## Q8
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** Qual é a diferença entre `AddScoped`, `AddTransient` e `AddSingleton` na injeção de dependência do ASP.NET Core, e quais problemas podem surgir do uso incorreto?
**Ideal Answer:** `Singleton` cria uma instância para o tempo de vida da aplicação. `Scoped` cria uma por requisição HTTP. `Transient` cria uma nova instância toda vez que é solicitada. Uma armadilha comum são "dependências cativas": injetar um serviço scoped ou transient em um singleton significa que o serviço de vida curta é efetivamente promovido ao tempo de vida de singleton, levando a estado obsoleto ou condições de corrida. O `DbContext` do EF Core é scoped e nunca deve ser injetado em um singleton. O ASP.NET Core lançará uma exceção na inicialização se isso for detectado com validação de escopo habilitada.

## Q9
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** Como middleware difere de filters no ASP.NET Core, e quando você usaria cada um?
**Ideal Answer:** Middleware opera no nível do pipeline HTTP e executa para toda requisição independente de roteamento. Ele lida com preocupações transversais como logging, CORS, autenticação e tratamento de exceções. Filters operam dentro do pipeline de ações do MVC/minimal API e têm acesso ao contexto da ação, resultado e estado do modelo — eles executam apenas para requisições que alcançam um endpoint. Use middleware para preocupações que se aplicam globalmente a todas as requisições. Use filters para preocupações vinculadas a ações de controllers (por exemplo, autorização em endpoints específicos, logging no nível da ação, transformação de resposta).

## Q10
**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** Explique como minimal APIs se comparam a APIs baseadas em controllers no ASP.NET Core. Quais são as implicações de desempenho?
**Ideal Answer:** Minimal APIs têm um overhead menor que APIs baseadas em controllers porque pulam o pipeline completo do MVC: sem model binding via reflexão em controllers, sem pipeline de action filter a menos que explicitamente adicionado, e um modelo de roteamento mais simples. Elas resultam em tempos de cold start mais rápidos e melhor compatibilidade com AOT. APIs baseadas em controllers fornecem mais convenções integradas (roteamento por atributos, action filters, comportamento `[ApiController]`). Para microsserviços de alto throughput, minimal APIs são preferidas. Para APIs grandes, mantidas por equipes, com políticas complexas de autorização e validação, controllers com filters podem ser mais manuteníveis.

## Q11
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** O que é `IOptions<T>`, `IOptionsSnapshot<T>` e `IOptionsMonitor<T>`? Quando você usaria cada um?
**Ideal Answer:** `IOptions<T>` é um singleton — a configuração é lida uma vez na inicialização e nunca muda. `IOptionsSnapshot<T>` é scoped — relê a configuração por requisição, útil em aplicações web onde a configuração pode mudar. `IOptionsMonitor<T>` é um singleton que suporta notificações de mudança e callbacks `OnChange`, ideal para serviços em background que precisam reagir a mudanças de configuração em tempo de execução. Use `IOptions<T>` para configuração estática, `IOptionsSnapshot<T>` quando atualização por requisição é necessária, e `IOptionsMonitor<T>` para serviços de longa execução.

## Q12
**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** Como o pipeline de requisições do ASP.NET Core lida com exceções? Compare `UseExceptionHandler`, `app.UseStatusCodePages` e uma abordagem de middleware customizado.
**Ideal Answer:** `UseExceptionHandler` captura exceções não tratadas lançadas em qualquer lugar abaixo no pipeline e re-executa no caminho de erro (por exemplo, `/error`). É a abordagem padrão de produção. `UseStatusCodePages` adiciona corpos de resposta para códigos de status sem corpo (por exemplo, 404, 405) — não captura exceções. Um middleware customizado envolvendo `next()` em um try/catch dá o máximo controle: você pode produzir uma resposta `ProblemDetails` consistente, logar com um ID de correlação e evitar o overhead de re-execução do `UseExceptionHandler`. A desvantagem do middleware customizado é maior responsabilidade de manutenção.

## Q13
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** Qual é o papel de `IHostedService` e `BackgroundService`? Como você implementaria um worker em background confiável?
**Ideal Answer:** `IHostedService` define os hooks de ciclo de vida `StartAsync` e `StopAsync`. `BackgroundService` é uma classe base que simplifica isso expondo `ExecuteAsync`. Para um worker confiável: sobrescreva `ExecuteAsync`, execute um loop com um `CancellationToken`, use `Task.Delay` ou um `PeriodicTimer` para intervalos, capture todas as exceções para evitar que o host desligue, e implemente desligamento gracioso respeitando o cancelamento. Para trabalho durável (por exemplo, processamento de filas), prefira usar um message broker com entrega at-least-once em vez de filas em memória.

## Q14
**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** O que é o problema de consulta N+1 no EF Core, e como você o resolve?
**Ideal Answer:** N+1 ocorre quando se carrega uma lista de N entidades e depois se carrega preguiçosamente uma entidade relacionada para cada uma, resultando em N+1 viagens de ida e volta ao banco de dados. Soluções no EF Core: (1) Carregamento eager com `.Include()` / `.ThenInclude()` — busca dados relacionados na mesma consulta usando JOINs. (2) Carregamento explícito — carregamento controlado após o fato. (3) Consultas divididas com `.AsSplitQuery()` — executa consultas separadas por navegação, mas evita explosão cartesiana em coleções. Evite lazy loading em APIs de produção, pois gera silenciosamente consultas excessivas.

## Q15
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** Qual é a diferença entre tracking do `SaveChanges` e `AsNoTracking`? Quando você usaria cada um?
**Ideal Answer:** Por padrão, o EF Core rastreia entidades retornadas de consultas no `ChangeTracker`. No `SaveChanges`, ele detecta mudanças e gera instruções UPDATE. `AsNoTracking()` pula o rastreamento — entidades não são registradas no change tracker, tornando leituras mais rápidas (menos memória, sem comparação de snapshot). Use tracking quando pretende modificar e salvar a entidade. Use `AsNoTracking` para consultas somente leitura (por exemplo, endpoints GET, modelos de leitura estilo Dapper) para melhorar o desempenho. `AsNoTrackingWithIdentityResolution` é um meio-termo: sem tracking, mas ainda resolve identidade de objetos.

## Q16
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** Como o EF Core lida com concorrência otimista? Qual é a abordagem da coluna de sistema `xmin` no PostgreSQL?
**Ideal Answer:** Concorrência otimista previne atualizações perdidas verificando que uma linha não mudou desde que foi lida. O EF Core marca uma propriedade como token de concorrência com `[ConcurrencyCheck]` ou `.IsRowVersion()`. No UPDATE/DELETE, o EF adiciona uma cláusula WHERE nesse token; se nenhuma linha é afetada, lança `DbUpdateConcurrencyException`. No SQL Server, `rowversion` é gerado automaticamente. No PostgreSQL, a abordagem `[Timestamp] byte[]` não funciona porque `bytea` não é gerado automaticamente. A abordagem correta no PostgreSQL usa a coluna de sistema `xmin` (tipo `xid`, mapeada para `uint` em C#) via `.HasColumnName("xmin").HasColumnType("xid").IsRowVersion()` — o Postgres atualiza automaticamente o `xmin` a cada escrita.

## Q17
**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** Qual é a diferença entre code-first e database-first no EF Core? Quais são os prós e contras de cada um?
**Ideal Answer:** Code-first: você define o modelo em C#, o EF gera migrações e o esquema. Prós: controle total do esquema no controle de versão, amigável para refatoração, migrações como código. Contras: esquemas complexos (por exemplo, stored procedures, indexação avançada) requerem edições manuais de migração. Database-first: você faz scaffold do modelo a partir de um banco existente usando `dotnet ef dbcontext scaffold`. Prós: útil ao integrar com um banco existente. Contras: o código gerado é verboso, difícil de customizar e diverge do banco conforme ambos evoluem. Para projetos greenfield, code-first é fortemente preferido.

## Q18
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** Como você implementaria um padrão de soft delete no EF Core sem modificar cada consulta na base de código?
**Ideal Answer:** Use um filtro de consulta global: defina uma coluna `IsDeleted` na entidade e configure `.HasQueryFilter(e => !e.IsDeleted)` na configuração da entidade. O EF Core automaticamente adiciona `WHERE is_deleted = false` a todas as consultas para aquela entidade. Para deletar permanentemente, use `.IgnoreQueryFilters()`. Combine com uma sobrescrita de `SaveChanges` que intercepta `EntityState.Deleted`, define `IsDeleted = true` e `DeletedAt = now`, e muda o estado para `Modified`. Isso torna o soft delete transparente em toda a base de código.

## Q19
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** Qual é a diferença entre CQRS e uma arquitetura em camadas tradicional? Quais problemas o CQRS resolve?
**Ideal Answer:** Em uma arquitetura em camadas, o mesmo modelo serve leituras e escritas, frequentemente levando a agregados complexos e over-fetching. CQRS (Command Query Responsibility Segregation) separa o modelo de escrita (comandos que mudam estado) do modelo de leitura (consultas que retornam dados). Benefícios: modelos de leitura podem ser otimizados independentemente (por exemplo, views desnormalizadas, Dapper em vez de EF), comandos podem impor invariantes em um modelo de domínio rico, e cada lado pode escalar independentemente. Também combina naturalmente com Event Sourcing. Desvantagem: mais código, consistência eventual em cenários distribuídos.

## Q20
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** O que é o Padrão Outbox, e como ele resolve o problema de escrita dupla em sistemas distribuídos?
**Ideal Answer:** O problema de escrita dupla: salvar no banco de dados e publicar um evento em um message broker em duas operações separadas pode deixá-los inconsistentes se uma falhar. O Padrão Outbox resolve isso escrevendo tanto a mudança de domínio quanto o evento na mesma transação do banco de dados (o evento vai para uma tabela `outbox`). Um processo em background separado consulta o outbox e publica eventos no broker, depois os marca como publicados. Isso garante entrega at-least-once sem eventos perdidos. Ferramentas como MassTransit (com outbox do EF Core) ou Debezium (baseado em CDC) implementam esse padrão.

## Q21
**Category:** Arquitetura
**Difficulty:** Medium
**Question:** Explique o padrão Repository. Quando ele agrega valor, e quando adiciona abstração desnecessária?
**Ideal Answer:** O padrão Repository abstrai o acesso a dados por trás de uma interface, habilitando testabilidade (mockar o repositório) e desacoplando o domínio da camada de dados. Ele agrega valor quando: a tecnologia de persistência provavelmente mudará, lógica complexa de consulta precisa ser centralizada, ou testes unitários sem banco real são necessários. Ele adiciona abstração desnecessária quando: o `DbContext` do EF Core (que já é uma unidade de trabalho + repositório) é envolvido em um repositório pass-through fino, adicionando boilerplate sem benefício. Em CQRS, command handlers frequentemente usam `DbContext` diretamente, enquanto modelos de leitura usam query handlers leves (por exemplo, Dapper), tornando um repositório genérico redundante.

## Q22
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** Quais são as vantagens e desvantagens entre uma arquitetura de microsserviços e um monolito modular?
**Ideal Answer:** Microsserviços: implantabilidade independente, heterogeneidade tecnológica, isolamento de falhas, escalabilidade horizontal por serviço. Custos: latência de rede, transações distribuídas, descoberta de serviços, complexidade operacional (Kubernetes, service mesh, rastreamento distribuído). Monolito modular: mesmo limite de implantação, mas com limites internos de módulo impostos (assemblies separados, sem acesso direto ao banco entre módulos). Mais fácil de desenvolver, testar e implantar. Menor overhead operacional. Ponto de partida recomendado para a maioria das equipes — você pode extrair serviços quando um limite é comprovado e a necessidade de escala é real. Decomposição prematura em microsserviços é um dos erros arquiteturais mais comuns.

## Q23
**Category:** Arquitetura
**Difficulty:** Medium
**Question:** O que é a Arquitetura Vertical Slice, e como ela difere de uma arquitetura em camadas tradicional (N-tier)?
**Ideal Answer:** Em N-tier (Controllers -> Services -> Repositories -> DB), cada funcionalidade toca todas as camadas horizontalmente. Vertical Slice organiza o código por funcionalidade: cada feature possui seu comando/consulta, handler e acesso a dados em uma pasta. Benefícios: menor acoplamento entre funcionalidades, mudanças são isoladas em um único slice, mais fácil de integrar novos membros (encontre tudo de uma feature em um só lugar), e cada slice pode escolher seu próprio padrão (alguns podem usar EF, outros SQL puro). A desvantagem é a potencial duplicação de código entre slices e a necessidade de disciplina para evitar que estado compartilhado se infiltre.

## Q24
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** Como você projetaria um endpoint de API idempotente? Por que a idempotência é importante, e como ela é tipicamente implementada?
**Ideal Answer:** Uma operação idempotente produz o mesmo resultado independentemente de quantas vezes é chamada. GET, PUT e DELETE são naturalmente idempotentes; POST não é. Para tornar um POST idempotente: exija que os clientes enviem um header `Idempotency-Key` (um UUID). O servidor armazena a chave e a resposta em um cache ou banco. Em uma requisição duplicada com a mesma chave, retorne a resposta armazenada sem re-executar. Isso é crítico para transações financeiras, criação de pedidos e qualquer operação com efeitos colaterais — permite retentativas seguras após falhas de rede sem duplicar estado.

## Q25
**Category:** Arquitetura
**Difficulty:** Medium
**Question:** O que é Domain-Driven Design (DDD)? Explique os conceitos de Aggregate, Entity, Value Object e Bounded Context.
**Ideal Answer:** DDD é uma abordagem de design de software que alinha a estrutura do código com o domínio de negócios. Entity: tem uma identidade única (por exemplo, `Order` identificado por `OrderId`). Value Object: definido por seus atributos, sem identidade, imutável (por exemplo, `Money`, `Address`). Aggregate: um cluster de entidades e value objects com uma entidade raiz que impõe invariantes — todo acesso passa pela raiz. Bounded Context: um limite explícito dentro do qual um modelo de domínio é consistente e uma linguagem ubíqua se aplica. Diferentes contextos podem modelar o mesmo conceito de forma diferente (por exemplo, "Cliente" em Faturamento vs. Envio). Bounded Contexts se comunicam via interfaces bem definidas (eventos, APIs).

## Q26
**Category:** Performance
**Difficulty:** Hard
**Question:** Como você diagnosticaria e corrigiria um endpoint de API lento em uma aplicação .NET em produção?
**Ideal Answer:** Comece com observabilidade: verifique traces distribuídos (por exemplo, OpenTelemetry + Jaeger) para identificar onde o tempo é gasto. Observe a duração das consultas ao banco — consultas N+1 e índices ausentes são as causas mais comuns. Use `EXPLAIN ANALYZE` em consultas lentas. Verifique alocações de memória com dotMemory ou `dotnet-counters` (pressão do GC pode causar picos de latência). Profile CPU com `dotnet-trace` e analise com PerfView ou SpeedScope. No código, procure por I/O síncrono bloqueando threads assíncronas, serialização excessiva ou alocações de objetos grandes. Corrija nesta ordem: consultas primeiro (índices, projeções), depois alocações, depois complexidade algorítmica.

## Q27
**Category:** Performance
**Difficulty:** Medium
**Question:** Qual é a diferença entre `IMemoryCache` e `IDistributedCache` no ASP.NET Core? Quando você usaria cada um?
**Ideal Answer:** `IMemoryCache` armazena dados na memória do processo — rápido, latência zero, mas não compartilhado entre múltiplas instâncias. Adequado para aplicações de instância única ou dados que podem diferir por instância (por exemplo, rate limiting local). `IDistributedCache` usa um armazenamento externo (Redis, SQL Server) compartilhado entre todas as instâncias — dados consistentes para implantações escaladas horizontalmente. Use `IMemoryCache` para cenários simples de servidor único. Use `IDistributedCache` (com Redis) em Kubernetes ou implantações multi-instância onde todos os nós devem ver os mesmos dados em cache (por exemplo, estado de sessão, rate limiting distribuído, lookups compartilhados).

## Q28
**Category:** Performance
**Difficulty:** Hard
**Question:** O que é o problema de starvation do ThreadPool em código assíncrono .NET, e como ele se manifesta?
**Ideal Answer:** O starvation do ThreadPool ocorre quando todas as threads disponíveis estão bloqueadas aguardando operações assíncronas completarem (tipicamente devido a `.Result` ou `.Wait()` em métodos assíncronos, ou I/O síncrono). Novo trabalho não pode executar porque nenhuma thread está livre, causando timeouts e requisições enfileiradas. Sintomas: alta latência de requisição sob carga, baixo uso de CPU, contagem de threads crescente. Correção: sempre use `await` e nunca bloqueie código assíncrono. Tenha especial cuidado em middleware, filters e código de bibliotecas. Use `async` em toda a cadeia de chamadas. Monitore com `dotnet-counters` observando `ThreadPool Queue Length` e `ThreadPool Thread Count`.

## Q29
**Category:** Performance
**Difficulty:** Medium
**Question:** Como `System.Text.Json` difere de `Newtonsoft.Json`, e quais são as vantagens e desvantagens de desempenho?
**Ideal Answer:** `System.Text.Json` é a biblioteca JSON integrada da Microsoft, projetada para desempenho: menos alocações, parsing baseado em `Span<byte>` e suporte a source generator para AOT. É significativamente mais rápido que Newtonsoft.Json para cenários comuns. Porém, tem menos funcionalidades: sem manipulação dinâmica de JSON via `JObject`, desserialização mais restrita por padrão (sem correspondência case-insensitive por padrão, sem tratamento de membros ausentes) e suporte limitado para cenários complexos como polimorfismo sem conversores customizados. Newtonsoft.Json é mais flexível e testado em batalha para cenários complexos. Para novos serviços, prefira `System.Text.Json` com source generators; recorra ao Newtonsoft.Json apenas para requisitos de serialização complexos.

## Q30
**Category:** Segurança
**Difficulty:** Medium
**Question:** Qual é a diferença entre autenticação e autorização no ASP.NET Core? Como funciona a autenticação baseada em JWT?
**Ideal Answer:** Autenticação estabelece quem é o usuário. Autorização determina o que ele tem permissão para fazer. No ASP.NET Core, `UseAuthentication()` executa primeiro e popula `HttpContext.User` com base nas credenciais fornecidas. `UseAuthorization()` então verifica as claims do usuário contra policies ou atributos `[Authorize]`. Autenticação JWT: o cliente envia um token assinado no header `Authorization: Bearer`. O middleware valida a assinatura usando a chave secreta do servidor, verifica expiração e claims de issuer/audience, e popula `ClaimsPrincipal`. Nenhuma sessão do lado do servidor é necessária — o token é autocontido. Expiração curta + refresh tokens é o padrão recomendado.

## Q31
**Category:** Segurança
**Difficulty:** Hard
**Question:** Quais são os riscos do OWASP Top 10 mais relevantes para APIs backend .NET? Como você os mitigaria?
**Ideal Answer:** Riscos principais para APIs .NET: (1) Controle de Acesso Quebrado — use `[Authorize]` com verificações no nível do recurso, nunca confie apenas em IDs fornecidos pelo cliente. (2) Injeção (SQL, comando) — use consultas parametrizadas (EF Core, Dapper), nunca concatene entrada do usuário em SQL. (3) Configuração de Segurança Incorreta — desabilite Swagger em produção, use HTTPS, valide parâmetros JWT estritamente. (4) Exposição de Dados Sensíveis — nunca logue tokens/senhas, criptografe PII em repouso. (5) Autenticação Quebrada — tempos de vida curtos para JWT, rotação de refresh tokens, revogação no logout. (6) SSRF — valide e use allowlist para quaisquer URLs que o servidor busca. (7) Atribuição em Massa — use DTOs em vez de vincular diretamente a entidades de domínio.

## Q32
**Category:** Segurança
**Difficulty:** Medium
**Question:** O que é rotação de refresh token, e por que é importante para fluxos OAuth?
**Ideal Answer:** Rotação de refresh token significa emitir um novo refresh token toda vez que o atual é usado para obter um novo access token, e invalidar o antigo. Isso limita a janela de exposição se um refresh token for roubado — o atacante só pode usá-lo uma vez antes de ser rotacionado. Implemente famílias de refresh tokens: se um token antigo (já rotacionado) for apresentado, isso sinaliza um possível roubo — invalide toda a família. Refresh tokens devem ser armazenados com hash no banco de dados (não em texto plano), ter uma expiração longa porém limitada (por exemplo, 30 dias), e ser vinculados a um identificador de dispositivo/sessão para segurança adicional.

## Q33
**Category:** Segurança
**Difficulty:** Hard
**Question:** Como você preveniria injeção SQL ao usar Dapper?
**Ideal Answer:** Sempre use consultas parametrizadas. No Dapper, passe parâmetros como um objeto anônimo: `db.QueryAsync<T>("SELECT * FROM users WHERE id = @Id", new { Id = id })`. Nunca use interpolação de string ou concatenação para construir SQL: `$"SELECT * FROM users WHERE id = {id}"` é vulnerável. Para ORDER BY dinâmico ou nomes de tabela (que não podem ser parametrizados), use uma whitelist de valores permitidos validados no código. O Dapper não fornece nenhum escape automático — o desenvolvedor é totalmente responsável pela parametrização. Também evite o método `Execute` com SQL multi-instrução fornecido pelo usuário.

## Q34
**Category:** Testes
**Difficulty:** Medium
**Question:** Qual é a diferença entre testes unitários, testes de integração e testes end-to-end? Como você decide o equilíbrio correto para uma API .NET?
**Ideal Answer:** Testes unitários: testam uma única classe ou função isoladamente, com todas as dependências mockadas. Rápidos, sem I/O. Testes de integração: testam múltiplos componentes juntos incluindo infraestrutura real (banco de dados, cliente HTTP). Mais lentos, mas capturam bugs reais de interação. Testes end-to-end: testam o sistema completo da perspectiva do cliente. Mais lentos e mais frágeis. Para APIs .NET, o equilíbrio recomendado (pirâmide de testes): muitos testes unitários para lógica de negócio (handlers de comando/consulta, serviços de domínio); testes de integração para consultas ao banco, middleware e comportamento de endpoints usando `WebApplicationFactory`; poucos testes end-to-end apenas para jornadas críticas do usuário.

## Q35
**Category:** Testes
**Difficulty:** Medium
**Question:** Como `WebApplicationFactory<T>` funciona em testes de integração do ASP.NET Core, e como você substitui serviços para teste?
**Ideal Answer:** `WebApplicationFactory<T>` cria um servidor de teste usando a inicialização real do `Program.cs`, permitindo testes de integração HTTP completos sem implantar a aplicação. Ele inicia um `TestServer` em memória e fornece um `HttpClient` para requisições. Para substituir serviços, sobrescreva `ConfigureWebHost` e chame `builder.ConfigureServices(services => services.AddSingleton<IMyService>(mock))`. Para bancos de dados, substitua as opções do `DbContext` com um banco em memória ou test-container. Essa abordagem testa o pipeline completo incluindo roteamento, middleware, autenticação e serialização, tornando-a o tipo de teste mais valioso para corretude de API.

## Q36
**Category:** Testes
**Difficulty:** Hard
**Question:** Quais são os riscos de usar bancos de dados mockados em testes de integração, e qual é a alternativa recomendada?
**Ideal Answer:** Bancos de dados mockados ou em memória (por exemplo, `UseInMemoryDatabase`) não impõem restrições SQL, não testam desempenho de consultas e diferem significativamente de um banco real em comportamento (sem transações, sem migrações, sem validações no nível SQL). Bugs que só se manifestam contra Postgres ou SQL Server reais não serão capturados. A alternativa recomendada é Testcontainers: inicie um container Docker real do banco alvo por execução de teste. A biblioteca `Testcontainers.PostgreSql` (ou `MsSql`) gerencia o ciclo de vida do container. Os testes executam contra um banco real, migrações são aplicadas e o container é destruído após a suíte de testes. Isso fornece alta confiança sem precisar de um banco de teste compartilhado.

## Q37
**Category:** Testes
**Difficulty:** Medium
**Question:** O que é FluentAssertions, e como ele melhora a legibilidade dos testes comparado às asserções integradas do xUnit?
**Ideal Answer:** FluentAssertions fornece uma API fluente, em linguagem natural, para asserções. Em vez de `Assert.Equal(expected, actual)`, você escreve `actual.Should().Be(expected)` ou `result.Should().HaveCount(3).And.Contain(x => x.Name == "Test")`. Quando um teste falha, FluentAssertions produz mensagens de erro detalhadas e legíveis mostrando os valores real vs. esperado incluindo estrutura do objeto. Suporta asserções ricas para coleções, exceções (`action.Should().Throw<ArgumentException>()`), datas, strings e código assíncrono. Reduz significativamente o tempo gasto depurando testes falhando.

## Q38
**Category:** Azure
**Difficulty:** Medium
**Question:** O que é Azure Service Bus, e como ele difere do Azure Storage Queues?
**Ideal Answer:** Azure Service Bus é um message broker empresarial que suporta tópicos/assinaturas (pub-sub), sessões de mensagem (processamento ordenado), filas de dead-letter, detecção de duplicatas, transações e entrega at-least-once. Storage Queues são mais simples, mais baratos e têm maior throughput, mas não possuem tópicos, sessões e transações. Use Service Bus quando precisar de: fan-out pub-sub, ordenação de mensagens, dead-lettering com políticas de retry, ou mensageria transacional. Use Storage Queues para cenários simples, de alto volume e sensíveis a custo onde funcionalidades avançadas de mensageria não são necessárias. Service Bus integra bem com MassTransit no .NET.

## Q39
**Category:** Azure
**Difficulty:** Hard
**Question:** O que é Azure Managed Identity, e como ela substitui strings de conexão para autenticação em recursos Azure?
**Ideal Answer:** Managed Identity fornece uma identidade Azure AD a um recurso Azure (por exemplo, App Service, pod AKS) sem armazenar credenciais. Atribuída ao sistema: vinculada ao ciclo de vida do recurso. Atribuída pelo usuário: independente, pode ser compartilhada entre recursos. Em vez de uma string de conexão com usuário/senha, a aplicação usa `DefaultAzureCredential` do Azure SDK, que automaticamente resolve o token de identidade do ambiente. Exemplo: conectar ao Azure SQL com `Authentication=Active Directory Managed Identity` na string de conexão, ou conectar ao Key Vault usando `new SecretClient(uri, new DefaultAzureCredential())`. Elimina segredos de arquivos de configuração e a carga de rotação.

## Q40
**Category:** Azure
**Difficulty:** Medium
**Question:** O que é Azure Key Vault, e como você o integra com o sistema de configuração de uma aplicação ASP.NET Core?
**Ideal Answer:** Azure Key Vault armazena segredos, certificados e chaves de criptografia com controle de acesso granular e logs de auditoria. Integração com ASP.NET Core: adicione o pacote NuGet `Azure.Extensions.AspNetCore.Configuration.Secrets` e chame `builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential())`. Os segredos do Key Vault ficam disponíveis via a interface padrão `IConfiguration`. Nomes de segredos usam `--` como separador para configuração aninhada (por exemplo, `ConnectionStrings--Default`). Combine com Managed Identity para que nenhuma credencial seja necessária para acessar o próprio vault. Em desenvolvimento, use `dotnet user-secrets` ou um arquivo `.env` local.

## Q41
**Category:** Azure
**Difficulty:** Hard
**Question:** Como o Azure API Management (APIM) complementa uma API backend .NET? Quais funcionalidades ele fornece?
**Ideal Answer:** O APIM fica na frente das APIs backend e fornece: rate limiting e throttling por assinatura/produto, autenticação (validação JWT, OAuth, certificados de cliente) antes das requisições chegarem ao backend, transformação de requisição/resposta via policies (adicionar headers, reescrever URLs), caching, logging no Application Insights, versionamento e um portal de desenvolvedor para documentação de API. Benefícios: delegar preocupações transversais do backend, proteger serviços de abuso e fornecer um ponto de entrada unificado para múltiplos serviços backend. Em microsserviços .NET, o APIM atua como um API gateway, reduzindo duplicação de código de preocupações comuns entre serviços.

## Q42
**Category:** Azure
**Difficulty:** Medium
**Question:** O que é Azure Application Insights, e como você o configura para rastreamento distribuído em uma aplicação .NET?
**Ideal Answer:** Application Insights é o serviço de APM do Azure. Para .NET, adicione `Microsoft.ApplicationInsights.AspNetCore` ou use OpenTelemetry com o exportador Azure Monitor. Configure com `builder.Services.AddApplicationInsightsTelemetry()` e defina a variável de ambiente `APPLICATIONINSIGHTS_CONNECTION_STRING`. Ele captura: telemetria de requisições, chamadas de dependência (HTTP, SQL), exceções, eventos customizados e métricas. Para rastreamento distribuído, usa o header W3C `traceparent` para correlacionar telemetria entre serviços. O Application Map mostra dependências de serviços. Use o `TelemetryClient` para eventos customizados ou o SDK OpenTelemetry para instrumentação vendor-neutral.

## Q43
**Category:** Azure
**Difficulty:** Hard
**Question:** Quais são as opções de implantação para uma API .NET no Azure? Compare Azure App Service, Azure Container Apps e AKS.
**Ideal Answer:** App Service: PaaS, modelo de implantação mais simples (zip deploy, GitHub Actions), auto-scaling, SSL integrado, sem necessidade de conhecimento de orquestração de containers. Melhor para aplicações web simples. Azure Container Apps: containers serverless, auto-scaling orientado a eventos (KEDA), integração com Dapr, sem gerenciamento de cluster Kubernetes. Melhor para microsserviços ou aplicações containerizadas sem expertise profunda em K8s. AKS (Azure Kubernetes Service): controle total do Kubernetes, workloads complexos, rede customizada, stateful sets. Maior custo operacional e complexidade. Melhor quando você precisa de agendamento avançado, operators customizados ou isolamento multi-tenant. Para a maioria das APIs .NET: App Service para apps simples, Container Apps para microsserviços container-first, AKS para plataformas complexas e de larga escala.

## Q44
**Category:** Azure
**Difficulty:** Medium
**Question:** O que é Azure Blob Storage, e como você o usa a partir de uma aplicação .NET para upload de arquivos?
**Ideal Answer:** Azure Blob Storage armazena dados não estruturados (arquivos, imagens, logs) em containers. A partir do .NET, use o SDK `Azure.Storage.Blobs`: crie um `BlobServiceClient` (autenticado via string de conexão ou `DefaultAzureCredential`), obtenha um `BlobContainerClient` e faça upload com `blobClient.UploadAsync(stream, overwrite: true)`. Para upload de arquivos grandes de clientes, prefira tokens SAS (Shared Access Signature): gere uma URL com tempo limitado no servidor e deixe o cliente fazer upload diretamente no Blob Storage, contornando sua API. Isso evita streaming de arquivos grandes através do servidor, reduzindo pressão de memória e custos de egress.

## Q45
**Category:** Azure
**Difficulty:** Hard
**Question:** O que é Azure Functions, e quando você o escolheria em vez de um `BackgroundService` hospedado no ASP.NET Core?
**Ideal Answer:** Azure Functions é um serviço de computação serverless que executa código em resposta a triggers (HTTP, timer, Service Bus, Blob, Event Grid). Escala até zero (sem custo quando ocioso), escala horizontalmente automaticamente e não tem gerenciamento de servidor. Escolha Functions quando: a carga de trabalho é orientada a eventos e esporádica, você quer cobrança por execução, ou quer implantabilidade isolada por função. Escolha `BackgroundService` no ASP.NET Core quando: você precisa de integração estreita com o container de DI da mesma aplicação, o trabalho é contínuo (por exemplo, polling), ou você precisa de latência previsível sem cold start. Cold starts do Functions podem ser mitigados com plano Premium ou pré-aquecimento.

## Q46
**Category:** Bancos de Dados
**Difficulty:** Hard
**Question:** Qual é a diferença entre um índice clusterizado e um não-clusterizado em bancos de dados SQL? Como o design de índices afeta o desempenho de APIs .NET?
**Ideal Answer:** Um índice clusterizado define a ordem física das linhas no disco — há um por tabela (a chave primária por padrão). Um índice não-clusterizado é uma estrutura separada com ponteiros para as linhas reais. Consultas que filtram ou ordenam em colunas de índice não-clusterizado evitam varreduras completas de tabela. Para desempenho de APIs .NET: identifique consultas lentas via logging do EF Core ou `EXPLAIN ANALYZE` (Postgres). Adicione índices não-clusterizados em colunas usadas em cláusulas WHERE, JOIN ON e ORDER BY. Índices de cobertura (incluem todas as colunas selecionadas) eliminam key lookups. Indexação excessiva prejudica o desempenho de escrita. Migrações do EF Core suportam `.HasIndex()` e `.IncludeProperties()` para índices compostos e de cobertura.

## Q47
**Category:** Bancos de Dados
**Difficulty:** Medium
**Question:** O que é uma transação de banco de dados, e como você gerencia transações no EF Core e Dapper dentro da mesma unidade de trabalho?
**Ideal Answer:** Uma transação agrupa múltiplas operações em uma unidade atômica — todas são bem-sucedidas ou todas são revertidas. No EF Core, `SaveChangesAsync()` envolve todas as mudanças pendentes em uma transação automaticamente. Para transações explícitas: `using var tx = await context.Database.BeginTransactionAsync()`. Para compartilhar uma transação entre EF Core e Dapper: obtenha o `DbConnection` e `DbTransaction` subjacentes do EF Core (`context.Database.GetDbConnection()`, `context.Database.CurrentTransaction.GetDbTransaction()`) e passe-os para os overloads `Execute`/`Query` do Dapper. Isso garante que tanto o ORM quanto SQL puro participem da mesma transação.

## Q48
**Category:** Bancos de Dados
**Difficulty:** Hard
**Question:** O que é connection pooling no contexto de acesso a banco de dados .NET, e como funciona com Npgsql/PostgreSQL?
**Ideal Answer:** Connection pooling reutiliza conexões de banco de dados estabelecidas em vez de criar uma nova conexão TCP por requisição, o que é custoso. O Npgsql tem um pool de conexões integrado por string de conexão. Quando `OpenAsync()` é chamado, o Npgsql retorna uma conexão do pool; quando `CloseAsync()` (ou `Dispose()`) é chamado, ele a devolve ao pool — sem realmente fechar a conexão TCP. Configurações principais: `Maximum Pool Size` (padrão 100), `Minimum Pool Size`, `Connection Idle Lifetime`. Com o `DbContext` scoped do EF Core, cada requisição obtém uma conexão do pool e a devolve no final do escopo. Configurar incorretamente o tamanho do pool em relação ao `max_connections` do Postgres pode causar exaustão de conexões.

## Q49
**Category:** Mensageria
**Difficulty:** Hard
**Question:** O que é MassTransit, e como ele simplifica a comunicação baseada em mensagens em microsserviços .NET?
**Ideal Answer:** MassTransit é uma abstração de mensageria .NET open-source que suporta RabbitMQ, Azure Service Bus, Amazon SQS e outros via uma API consistente. Ele lida com: registro de consumidores (integrado com DI), políticas de retry, circuit breakers, filas de dead-letter, sagas (orquestração de máquina de estado para workflows distribuídos), request/response sobre mensageria, e o Padrão Outbox com EF Core. Em vez de escrever código específico do broker, você define `IConsumer<TMessage>` e o registra com `services.AddMassTransit(x => x.AddConsumer<MyConsumer>())`. Ele reduz significativamente o boilerplate para mensageria confiável e integra com os hosted services do ASP.NET Core.

## Q50
**Category:** Mensageria
**Difficulty:** Medium
**Question:** Qual é a diferença entre mensageria ponto-a-ponto e publish/subscribe? Dê um exemplo concreto com Azure Service Bus.
**Ideal Answer:** Ponto-a-ponto (fila): uma mensagem é enviada para uma fila nomeada e consumida por exatamente um receptor. Adequado para distribuição de trabalho (por exemplo, uma fila de jobs onde um worker processa cada pedido). Publish/subscribe (tópico + assinatura): um publicador envia uma mensagem para um tópico; múltiplas assinaturas (cada uma com suas próprias regras de filtro) recebem cópias independentes. Adequado para integração orientada a eventos (por exemplo, evento `OrderPlaced` consumido pelo serviço de Faturamento e pelo serviço de Notificação independentemente). No Azure Service Bus: use `QueueClient` para P2P, use `TopicClient` + `SubscriptionClient` para pub-sub. MassTransit abstrai ambos os padrões.

## Q51
**Category:** Padrões de Design
**Difficulty:** Medium
**Question:** O que é o padrão Mediator, e como o MediatR o implementa no ASP.NET Core?
**Ideal Answer:** O padrão Mediator centraliza a comunicação entre objetos através de um único mediador, reduzindo acoplamento direto. O MediatR implementa isso fazendo todos os comandos e consultas implementarem `IRequest<TResponse>`. Handlers implementam `IRequestHandler<TRequest, TResponse>` e são descobertos via DI. O endpoint chama `mediator.Send(new MyCommand(...))` sem saber qual handler o processa. Isso desacopla a camada HTTP da lógica de negócio, torna handlers testáveis independentemente e suporta comportamentos transversais (pipeline behaviors) para logging, validação e caching inseridos entre a requisição e o handler.

## Q52
**Category:** Padrões de Design
**Difficulty:** Medium
**Question:** O que é o padrão Decorator, e como ele pode ser aplicado em .NET usando DI para adicionar comportamento transversal?
**Ideal Answer:** O Decorator envolve um objeto para adicionar comportamento sem modificá-lo. No DI do .NET, registre decorators usando Scrutor (`services.Decorate<IMyService, LoggingMyService>()`) ou manualmente resolvendo o serviço interno e envolvendo-o. Exemplo: um `CachingQueryHandler<T>` que envolve um handler real e retorna resultados do cache. Pipeline behaviors no MediatR alcançam o mesmo efeito para requisições. Benefícios: responsabilidade única (cada decorator faz uma coisa), aberto/fechado (adicione comportamento sem modificar código existente), testável isoladamente. Casos de uso comuns: logging, caching, retry, verificações de autorização.

## Q53
**Category:** Padrões de Design
**Difficulty:** Hard
**Question:** O que é o padrão Specification, e como ele ajuda na composição de consultas complexas no EF Core?
**Ideal Answer:** Uma Specification encapsula critérios de consulta como um objeto. Em vez de passar predicados brutos por toda parte, você define `class ActiveUserSpecification : Specification<User>` que constrói uma `Expression<Func<User, bool>>`. O repositório ou query handler aplica a specification: `context.Users.Where(spec.Criteria)`. Benefícios: reutilizável, combinável (operadores AND/OR), testável sem banco de dados. Bibliotecas como Ardalis.Specification fornecem uma classe base e `ISpecificationEvaluator` que também lida com `Include`, `OrderBy` e paginação. Isso evita que lógica de consulta vaze para handlers e controllers.

## Q54
**Category:** Padrões de Design
**Difficulty:** Medium
**Question:** O que é o padrão Factory? Quando você usaria `IServiceProvider` como factory vs. uma classe factory dedicada?
**Ideal Answer:** O padrão Factory encapsula a criação de objetos. No .NET, `IServiceProvider.GetRequiredService<T>()` atua como um service locator/factory, mas é um antipadrão quando usado em lógica de negócio (esconde dependências). Uma interface factory dedicada (`INotificationFactory`) torna dependências explícitas e é mais fácil de testar. Use `IServiceProvider` como factory apenas em código de infraestrutura (por exemplo, criando serviços scoped dentro de um singleton como um hosted service: `scope = provider.CreateScope()`). Use uma classe factory dedicada para criar objetos de domínio ou quando o tipo a ser criado depende de dados em tempo de execução.

## Q55
**Category:** Logging e Observabilidade
**Difficulty:** Medium
**Question:** O que são logs estruturados, e por que são preferíveis a logs em texto plano em uma aplicação cloud-native?
**Ideal Answer:** Logs estruturados armazenam dados de log como pares chave-valor (JSON) em vez de strings de texto livre. Em vez de `"User 42 logged in from 1.2.3.4"`, você emite `{ "event": "UserLogin", "userId": 42, "ip": "1.2.3.4" }`. Isso permite: filtragem e consulta por campos específicos (por exemplo, todos os eventos para `userId = 42`), dashboards de agregações e métricas, e correlação de eventos entre serviços. No .NET, Serilog e Microsoft.Extensions.Logging suportam logging estruturado. Com Serilog, use templates de mensagem: `Log.Information("User {UserId} logged in from {IpAddress}", userId, ip)`. Os valores são capturados como propriedades estruturadas, não apenas formatados na string.

## Q56
**Category:** Logging e Observabilidade
**Difficulty:** Hard
**Question:** O que é OpenTelemetry, e como ele unifica rastreamento, métricas e logging em uma aplicação .NET?
**Ideal Answer:** OpenTelemetry (OTel) é um framework de observabilidade vendor-neutral que fornece um único conjunto de APIs e SDKs para traces, métricas e logs. No .NET: adicione `OpenTelemetry.Extensions.Hosting`, configure `TracerProvider` com `AddAspNetCoreInstrumentation()`, `AddHttpClientInstrumentation()`, `AddEntityFrameworkCoreInstrumentation()`, e exporte para Jaeger, Zipkin ou Azure Monitor via `AddOtlpExporter()`. Traces se propagam via headers W3C `traceparent` entre serviços, habilitando correlação de requisições end-to-end. Métricas são exportadas para Prometheus. Isso substitui SDKs proprietários (SDK Application Insights, tracer Datadog) por uma única camada de instrumentação, permitindo trocar backends sem mudanças de código.

## Q57
**Category:** Logging e Observabilidade
**Difficulty:** Medium
**Question:** O que é um ID de correlação, e como você o propagaria entre serviços e o logaria consistentemente?
**Ideal Answer:** Um ID de correlação é um identificador único anexado a uma requisição e passado por todas as chamadas de serviço e entradas de log, permitindo rastrear uma única requisição de usuário através de múltiplos serviços e linhas de log. Implementação no ASP.NET Core: middleware lê `X-Correlation-ID` dos headers recebidos (ou gera um novo), armazena em um serviço scoped ou `Activity.Current`, e o adiciona a requisições `HttpClient` de saída via um `DelegatingHandler`. No Serilog, use `LogContext.PushProperty("CorrelationId", id)` via enriquecimento do `UseSerilogRequestLogging` para que cada linha de log naquele escopo de requisição inclua o ID. O `TraceId` do OpenTelemetry serve ao mesmo propósito.

## Q58
**Category:** Design de API
**Difficulty:** Medium
**Question:** O que é ProblemDetails (RFC 7807), e como você o implementa consistentemente em uma API ASP.NET Core?
**Ideal Answer:** ProblemDetails é um formato padrão HTTP para respostas de erro legíveis por máquina, incluindo campos `type`, `title`, `status`, `detail` e `instance`. No ASP.NET Core 7+, chame `builder.Services.AddProblemDetails()` e use `Results.Problem(...)` em minimal APIs ou `Problem(...)` em controllers. Para exceções não tratadas, configure `UseExceptionHandler` para retornar `ProblemDetails` via `IExceptionHandler`. Isso dá aos clientes um contrato de erro consistente independente do endpoint, e permite propriedades de extensão (por exemplo, `errors` para falhas de validação). É preferível a respostas ad-hoc `{ "message": "..." }` que variam por endpoint.

## Q59
**Category:** Design de API
**Difficulty:** Medium
**Question:** O que é versionamento de API, e como você o implementaria em uma aplicação ASP.NET Core?
**Ideal Answer:** O versionamento de API permite evoluir uma API sem quebrar clientes existentes. Estratégias comuns: caminho da URL (`/api/v1/users`), query string (`?api-version=1.0`) ou header (`Api-Version: 1.0`). Use o pacote NuGet `Asp.Versioning.Http`: chame `builder.Services.AddApiVersioning(options => options.DefaultApiVersion = new ApiVersion(1, 0))`. Anote endpoints ou controllers com `[ApiVersion("1.0")]`. Deprecie versões antigas com `[ApiVersion("1.0", Deprecated = true)]`. Documente com Swagger usando `Asp.Versioning.Mvc.ApiExplorer`. Prefira versionamento baseado em header para APIs que devem manter URLs limpas; versionamento baseado em URL é mais visível e mais fácil de testar manualmente.

## Q60
**Category:** Design de API
**Difficulty:** Hard
**Question:** O que é HATEOAS, e é prático implementá-lo em APIs REST .NET modernas?
**Ideal Answer:** HATEOAS (Hypermedia as the Engine of Application State) significa que as respostas da API incluem links para ações e recursos relacionados, permitindo que clientes naveguem a API dinamicamente sem codificar URLs fixas. Exemplo: uma resposta `GET /orders/1` inclui `"links": [{"rel": "cancel", "href": "/orders/1/cancel", "method": "POST"}]`. Em teoria, desacopla cliente e servidor. Na prática: a maioria das APIs do mundo real (incluindo grandes provedores de nuvem) não implementa HATEOAS completo porque os clientes precisam entender a semântica dos recursos de qualquer forma, e o overhead de geração de links não se justifica. Raramente é implementado além do Nível 2 de Maturidade Richardson. Uma especificação OpenAPI bem documentada é tipicamente mais prática.

## Q61
**Category:** Concorrência
**Difficulty:** Hard
**Question:** Qual é a diferença entre concorrência otimista e pessimista, e quando você escolheria cada uma em uma API .NET?
**Ideal Answer:** Concorrência otimista: assume que conflitos são raros. Lê dados, processa, então ao salvar verifica que a linha não mudou (via `rowversion`/`xmin`). Se mudou, lança `DbUpdateConcurrencyException` e deixa o cliente retentar. Baixo overhead, sem locks mantidos. Concorrência pessimista: bloqueia a linha na leitura para que nenhuma outra transação possa modificá-la (`SELECT FOR UPDATE` no Postgres). Garante nenhum conflito, mas mantém locks, reduzindo throughput e arriscando deadlocks. Use otimista para a maioria dos cenários de API web (baixa contenção, transações curtas). Use pessimista para cenários de alta contenção onde o custo da resolução de conflito é alto (por exemplo, reserva de assentos, dedução de estoque).

## Q62
**Category:** Concorrência
**Difficulty:** Hard
**Question:** Como `SemaphoreSlim` difere de `lock` para código assíncrono, e quando você usaria cada um?
**Ideal Answer:** `lock` é síncrono — bloqueia a thread enquanto aguarda adquirir o monitor. Usar `lock` em código assíncrono é perigoso porque você não pode usar await dentro de um bloco lock, e o bloqueio pode causar starvation do ThreadPool. `SemaphoreSlim` suporta espera assíncrona: `await semaphore.WaitAsync()` libera a thread enquanto aguarda, permitindo que outro trabalho prossiga. Use `lock` para seções críticas síncronas muito curtas (por exemplo, atualizando uma coleção em memória). Use `SemaphoreSlim(1, 1)` como um mutex assíncrono para proteger estado compartilhado em código assíncrono. Use `SemaphoreSlim(N, N)` para limitar acesso concorrente a um recurso (por exemplo, máximo de 5 chamadas HTTP simultâneas a uma API externa).

## Q63
**Category:** Concorrência
**Difficulty:** Medium
**Question:** O que é `CancellationToken`, e como ele deve ser propagado em uma API .NET?
**Ideal Answer:** `CancellationToken` sinaliza que uma operação deve ser cancelada (por exemplo, cliente desconecta, timeout da requisição). No ASP.NET Core, o framework injeta um `CancellationToken` nos parâmetros do endpoint/ação automaticamente — ele é cancelado quando o cliente desconecta. Propague-o por toda chamada assíncrona: consultas ao banco (`QueryAsync(..., cancellationToken: ct)`), chamadas de cliente HTTP, I/O de arquivo e trabalho em background. Nunca o ignore. Crie tokens vinculados para adicionar um timeout: `using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct); cts.CancelAfter(TimeSpan.FromSeconds(5))`. Propagação adequada previne vazamentos de recursos e execução de consultas no banco após o cliente ter desconectado.

## Q64
**Category:** Injeção de Dependência
**Difficulty:** Hard
**Question:** O que é o antipadrão Service Locator, e como ele difere do uso legítimo de `IServiceProvider`?
**Ideal Answer:** O padrão Service Locator significa chamar `serviceProvider.GetService<T>()` de dentro da lógica de negócio para resolver dependências em tempo de execução em vez de declará-las como parâmetros de construtor. É um antipadrão porque: dependências ficam ocultas (sem contrato explícito no construtor), a classe é mais difícil de testar (precisa configurar o container), e acopla lógica de negócio à infraestrutura de DI. Usos legítimos de `IServiceProvider`: criar serviços scoped dentro de um singleton (por exemplo, `provider.CreateScope()` em um `BackgroundService`), métodos factory em código de infraestrutura, e middleware que precisa resolver um serviço request-scoped. Em código de domínio e aplicação, sempre use injeção por construtor.

## Q65
**Category:** Injeção de Dependência
**Difficulty:** Medium
**Question:** Quais são as limitações da injeção por construtor no DI do .NET, e como você lida com dependências opcionais ou condicionais?
**Ideal Answer:** Injeção por construtor requer que todas as dependências estejam registradas e disponíveis. Para dependências opcionais: use `IServiceProvider.GetService<T>()` (retorna null se não registrado) em vez de `GetRequiredService<T>()`. Alternativamente, declare o parâmetro como nullable e use `null` como padrão. Para dependências condicionais (diferentes implementações por condição em tempo de execução): use um padrão factory — injete `IServiceProvider` ou uma factory `Func<T>`. Para genéricos abertos (por exemplo, `IRepository<T>`), registre com `services.AddScoped(typeof(IRepository<>), typeof(Repository<>))`. Evite over-engineering: a maioria dos serviços deve ter uma única implementação concreta.

## Q66
**Category:** Runtime .NET
**Difficulty:** Hard
**Question:** Como funciona o garbage collector do .NET? O que são Gen 0, Gen 1 e Gen 2, e o que é o Large Object Heap?
**Ideal Answer:** O .NET usa um GC geracional. Gen 0: objetos recém-alocados de vida curta. Coletado frequentemente, rápido (milissegundos). Gen 1: objetos que sobreviveram à Gen 0. Gen 2: objetos de vida longa (singletons, caches). Coletado infrequentemente, pode pausar a aplicação. Large Object Heap (LOH): objetos com 85.000 bytes ou mais são alocados aqui e só coletados com Gen 2. O LOH não é compactado por padrão, causando fragmentação. Para minimizar pressão do GC: prefira `Span<T>` e `stackalloc` para buffers temporários, use `ArrayPool<byte>.Shared` para arrays grandes, evite concatenações frequentes de strings grandes (use `StringBuilder`) e evite manter coleções grandes na Gen 2 se possível.

## Q67
**Category:** Runtime .NET
**Difficulty:** Medium
**Question:** Qual é a diferença entre `IDisposable` e `IAsyncDisposable`? Quando você deve implementar cada um?
**Ideal Answer:** `IDisposable.Dispose()` libera recursos não gerenciados de forma síncrona. `IAsyncDisposable.DisposeAsync()` libera recursos de forma assíncrona (por exemplo, fazendo flush de um stream assíncrono, fechando uma conexão de rede graciosamente). Implemente `IDisposable` quando sua classe mantém referências a recursos como handles de arquivo, memória não gerenciada ou conexões de banco. Implemente `IAsyncDisposable` quando a limpeza envolve I/O. Implemente ambos se a classe pode ser descartada em qualquer contexto. Use `await using` para `IAsyncDisposable`. Nunca chame `Dispose()` em um objeto ainda rastreado pelo container de DI — deixe o container gerenciar o tempo de vida. O container de DI do ASP.NET Core chama `DisposeAsync()` para serviços scoped/transient que o implementam.

## Q68
**Category:** Runtime .NET
**Difficulty:** Hard
**Question:** O que é Native AOT no .NET, e quais restrições ele impõe ao código da aplicação?
**Ideal Answer:** Native AOT (Ahead-of-Time compilation) compila código .NET para um binário nativo antes da implantação, eliminando o compilador JIT em tempo de execução. Benefícios: tempo de inicialização mais rápido (milissegundos vs. centenas de milissegundos), menor footprint de memória, implantação menor (sem runtime necessário), melhor cold start em containers. Restrições: sem geração de código em tempo de execução (sem `Reflection.Emit`, sem expression trees compiladas em runtime), reflexão limitada (deve usar source generators), sem carregamento dinâmico de assemblies, sem padrões `Type.GetType(string)`. Minimal APIs do ASP.NET Core e `System.Text.Json` com source generators são projetados para compatibilidade com AOT. O EF Core tem suporte AOT limitado. Mais adequado para funções Lambda, microsserviços e ferramentas CLI.

## Q69
**Category:** Runtime .NET
**Difficulty:** Medium
**Question:** Qual é a diferença entre interning de `string`, `StringBuilder` e `string.Create` em termos de alocação de memória?
**Ideal Answer:** `string` é imutável — cada concatenação cria um novo objeto string. String interning agrupa literais idênticos para reutilizar a mesma referência (`string.Intern`), útil para reduzir alocações com valores repetidos, mas arriscado para vazamentos de memória se usado excessivamente. `StringBuilder` acumula caracteres em um buffer mutável e só aloca a string final no `ToString()` — ideal para construir strings em loops. `string.Create(length, state, spanAction)` é o mais eficiente em alocação: aloca exatamente uma string e escreve nela via um `Span<char>`, evitando cópias intermediárias. Use `StringBuilder` para construção de comprimento variável; `string.Create` para caminhos de comprimento fixo e alta performance.

## Q70
**Category:** Cloud Native
**Difficulty:** Hard
**Question:** O que é a metodologia 12-Factor App, e como os fatores mais relevantes se aplicam a um microsserviço .NET?
**Ideal Answer:** 12-Factor define boas práticas para aplicações cloud-native. Mais relevantes para .NET: (1) Config: armazene configuração em variáveis de ambiente, nunca no código. Use `IConfiguration` + `AddEnvironmentVariables()`. (2) Serviços de apoio: trate banco, fila, cache como recursos anexados via configuração, não hardcoded. (3) Processos: seja stateless — sem sessões em memória, sem sticky sessions. (4) Logs: trate logs como streams de eventos — escreva para stdout/stderr, não em arquivos. Use logging estruturado. (5) Descartabilidade: inicialização rápida (< 5s), desligamento gracioso no SIGTERM. Implemente `IHostedService.StopAsync`. (6) Paridade dev/prod: use Docker Compose localmente para espelhar produção. (7) Dependências: declare todas as dependências no `.csproj`, sem dependências implícitas no nível do sistema.

## Q71
**Category:** Cloud Native
**Difficulty:** Medium
**Question:** O que é health checking no ASP.NET Core, e como você configuraria liveness vs. readiness probes para Kubernetes?
**Ideal Answer:** O ASP.NET Core fornece `AddHealthChecks()` e `MapHealthChecks()`. Liveness probe: verifica se o processo está vivo e não travado. Deve ser muito leve — apenas retorne Healthy. Mapeie para `/health/live` e verifique apenas si mesmo: `.AddCheck("self", () => HealthCheckResult.Healthy())`. Readiness probe: verifica se a aplicação está pronta para receber tráfego (banco conectado, dependências ativas). Mapeie para `/health/ready` e inclua todas as verificações: `.AddDbContextCheck<AppDbContext>()`. No Kubernetes, configure `livenessProbe` em `/health/live` e `readinessProbe` em `/health/ready`. Separe-os para evitar matar um pod que está apenas temporariamente incapaz de alcançar o banco.

## Q72
**Category:** Cloud Native
**Difficulty:** Hard
**Question:** Como você implementaria circuit breaking e políticas de retry em uma aplicação .NET chamando APIs HTTP externas?
**Ideal Answer:** Use Polly (ou `Microsoft.Extensions.Http.Resilience` no .NET 8+). Política de retry: retente em erros transientes (5xx, timeouts de rede) com backoff exponencial + jitter para evitar thundering herd. Circuit breaker: após N falhas consecutivas, abra o circuito por uma duração e falhe rápido sem chamar o serviço — previne falhas em cascata e permite recuperação. Configure via `services.AddHttpClient<MyClient>().AddResilienceHandler(...)` ou `AddTransientHttpErrorPolicy`. Considerações importantes: não retente operações não-idempotentes (POST) a menos que o endpoint seja idempotente. Logue transições de estado do circuito. Combine com políticas de timeout. Em sistemas distribuídos, resiliência não é opcional.

## Q73
**Category:** Cloud Native
**Difficulty:** Medium
**Question:** O que é containerização, e quais são as boas práticas para escrever um Dockerfile para uma API .NET?
**Ideal Answer:** Boas práticas para Dockerfiles .NET: (1) Use builds multi-estágio — imagem SDK para build, imagem runtime para final (reduz tamanho de ~700MB para ~200MB). (2) Restaure pacotes antes de copiar código-fonte para que o Docker faça cache da camada de restore. (3) Use a imagem `mcr.microsoft.com/dotnet/aspnet` (não SDK) para o estágio runtime. (4) Execute como usuário não-root para segurança. (5) Defina `ASPNETCORE_URLS=http://+:8080` para evitar executar na porta 80. (6) Use `.dockerignore` para excluir `bin/`, `obj/`, `.git/`. (7) Para apps compiladas com AOT, use a imagem base `chiseled` do Ubuntu para superfície de ataque mínima.

## Q74
**Category:** Habilidades Interpessoais e Processo
**Difficulty:** Medium
**Question:** Como você aborda uma revisão de pull request como desenvolvedor sênior? O que você procura além da corretude?
**Ideal Answer:** Além da corretude: (1) Design: a mudança introduz complexidade desnecessária? Poderia ser mais simples? É consistente com a arquitetura existente? (2) Segurança: algum risco de injeção, verificações de autorização ausentes, segredos em logs? (3) Performance: consultas N+1, coleções sem limites, índices ausentes, síncrono sobre assíncrono? (4) Testabilidade: as mudanças estão testadas? Os testes testam comportamento ou detalhes de implementação? (5) Tratamento de erros: erros são tratados graciosamente ou silenciosamente engolidos? (6) Observabilidade: eventos importantes são logados com contexto? (7) Nomenclatura e legibilidade: um novato entenderia isso daqui 6 meses? (8) Mudanças que quebram: isso afeta clientes ou contratos existentes?

## Q75
**Category:** Habilidades Interpessoais e Processo
**Difficulty:** Medium
**Question:** Como você lida com dívida técnica em um produto que se move rápido? Como você decide quando refatorar vs. entregar?
**Ideal Answer:** Dívida técnica é inevitável; a chave é gerenciá-la intencionalmente. Estratégias: (1) Rastreie dívida como itens explícitos de backlog, não conhecimento invisível. (2) Aplique a Regra do Escoteiro: deixe o código um pouco melhor do que encontrou. (3) Refatore no contexto de uma funcionalidade relacionada — não crie PRs de limpeza não relacionados que travam o review. (4) Use o padrão Strangler Fig para refatorações grandes: construa comportamento novo ao lado do antigo e migre incrementalmente. (5) Distinga entre dívida deliberada (atalho consciente com um plano) e dívida imprudente (sem plano). Entregue quando a dívida não bloqueia o objetivo atual e está rastreada. Refatore quando a dívida está ativamente desacelerando a equipe ou introduzindo bugs.

## Q76
**Category:** Habilidades Interpessoais e Processo
**Difficulty:** Hard
**Question:** Como você abordaria a migração de um monolito legado .NET Framework para .NET 8+ sem uma reescrita completa?
**Ideal Answer:** Padrão Strangler Fig: substitua incrementalmente partes do monolito em vez de reescrever tudo de uma vez. Passos: (1) Identifique uma fatia delimitada (por exemplo, um único subsistema) que pode ser extraída com dependências mínimas. (2) Crie um novo projeto ASP.NET Core ao lado do monolito. (3) Use um API Gateway ou proxy reverso (YARP, NGINX) para rotear tráfego daquela fatia para o novo serviço. (4) Compartilhe o banco de dados inicialmente (antipadrão, mas pragmático); extraia para esquema/banco separado ao longo do tempo. (5) Migre bibliotecas compartilhadas para `netstandard2.0` primeiro por compatibilidade, depois para `net8.0`. Evite reescrever lógica de negócio do zero — porte-a, mantenha os testes passando.

## Q77
**Category:** Design de API
**Difficulty:** Medium
**Question:** Qual é a diferença entre REST e gRPC? Quando você escolheria gRPC para um serviço .NET?
**Ideal Answer:** REST usa HTTP/1.1 com JSON, é legível por humanos e é o padrão para APIs públicas. gRPC usa HTTP/2 com Protocol Buffers (binário), fornecendo: menor tamanho de payload, streaming bidirecional, geração de código a partir de contratos `.proto` e contratos de API rigorosos. No .NET, use `Grpc.AspNetCore`. Escolha gRPC para: comunicação interna de alta throughput entre serviços, microsserviços poliglotas precisando de contratos fortemente tipados, streaming bidirecional (por exemplo, telemetria em tempo real) e clientes mobile onde largura de banda importa. Escolha REST para: APIs públicas, clientes de navegador (gRPC-Web adiciona complexidade) ou quando legibilidade humana e ferramentas (Postman, Swagger) são prioridades.

## Q78
**Category:** Design de API
**Difficulty:** Hard
**Question:** Como você projetaria uma API de paginação? Compare paginação baseada em offset vs. baseada em cursor e seus prós e contras.
**Ideal Answer:** Baseada em offset (`?page=2&pageSize=20`): simples de implementar e suporta pular para páginas arbitrárias. Desvantagem: resultados inconsistentes quando dados são inseridos/deletados durante a paginação (linhas se deslocam), e `OFFSET N` em SQL se torna lento para N grande (banco varre todas as linhas anteriores). Baseada em cursor: a resposta inclui um cursor (token opaco codificando o último ID visto ou chave de ordenação). A próxima requisição passa `?cursor=...` para buscar linhas após aquele cursor. Benefícios: resultados consistentes independente de escritas concorrentes, tempo de busca O(1) com o índice correto. Desvantagem: não pode pular para páginas arbitrárias, mais difícil de implementar. Use offset para grids administrativas com dados moderados; use cursor para feeds, streams de eventos e tabelas grandes.

## Q79
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** O que é Event Sourcing, e quais são as vantagens e desvantagens comparado a um modelo de persistência baseado em estado tradicional?
**Ideal Answer:** Event Sourcing persiste a sequência de eventos de domínio que levaram ao estado atual, em vez do estado atual em si. Reconstrói o estado reproduzindo eventos. Benefícios: log de auditoria completo, consultas de viagem no tempo, capacidade de projetar estado em múltiplos modelos de leitura, encaixe natural com CQRS. Desvantagens: consultas requerem projeções (modelos de leitura) — você não pode simplesmente `SELECT * FROM orders`. Evolução de esquema de eventos é complexa. Consistência eventual entre modelos de escrita e leitura. Reconstruir estado a partir de muitos eventos pode ser lento sem snapshots. Alta complexidade operacional. Mais adequado para domínios com requisitos inerentes de auditoria (financeiro, compliance) ou onde o histórico de eventos é uma funcionalidade de primeira classe. Excessivo para aplicações CRUD simples.

## Q80
**Category:** Arquitetura
**Difficulty:** Medium
**Question:** O que é o padrão Anti-Corruption Layer (ACL) em DDD, e quando é necessário?
**Ideal Answer:** Um ACL é uma camada de tradução entre dois bounded contexts ou sistemas com modelos diferentes. Ao integrar com um sistema legado, API externa ou serviço de terceiros, os conceitos do modelo externo não devem vazar para seu modelo de domínio. O ACL traduz DTOs externos, eventos ou estruturas para a linguagem do seu domínio. Exemplo: um gateway de pagamento externo retorna valores de `TransactionStatus` em seu próprio formato; o ACL mapeia estes para o enum `PaymentStatus` do seu domínio. Sem um ACL, mudanças no modelo externo forçam mudanças em todo o seu domínio. Implemente como adapters, mappers ou um serviço de tradução dedicado. Sempre use um ACL quando o modelo externo está fora do seu controle.

## Q81
**Category:** Azure
**Difficulty:** Hard
**Question:** O que é Azure Event Grid, e como ele difere do Azure Service Bus e do Azure Event Hubs?
**Ideal Answer:** Event Grid: roteamento reativo de eventos para eventos de recursos Azure e eventos customizados. Baixa latência, push HTTP (webhooks), serverless. Melhor para reagir a mudanças em recursos Azure (Blob criado, resource group deletado) ou eventos customizados leves. Sem ordenação ou replay. Service Bus: message broker empresarial. Processamento ordenado, dead-lettering, transações, sessões. Melhor para processamento confiável e ordenado de comandos entre serviços. Event Hubs: streaming de eventos de alto throughput (milhões de eventos/segundo). Retém eventos por 24h-90 dias, suporta consumer groups para processamento paralelo, integra com protocolo Apache Kafka. Melhor para telemetria, agregação de logs, pipelines de analytics em tempo real. A escolha depende do volume, necessidades de ordenação e modelo de consumidor.

## Q82
**Category:** Azure
**Difficulty:** Medium
**Question:** O que é Azure Redis Cache, e como você implementaria um cache distribuído em uma API ASP.NET Core?
**Ideal Answer:** Azure Cache for Redis é uma instância Redis gerenciada. Integre com ASP.NET Core via `services.AddStackExchangeRedisCache(options => options.Configuration = config["Redis:ConnectionString"])`. Use `IDistributedCache.GetAsync/SetAsync` com serialização em array de bytes, ou envolva com um helper tipado usando `System.Text.Json`. Para cache fortemente tipado, use `StackExchange.Redis` diretamente para operações mais ricas (sets, sorted sets, pub-sub). Padrões comuns: cache-aside (verifique cache, em caso de miss carregue do banco e popule cache com expiração), write-through (atualize banco e cache atomicamente). Defina TTLs apropriados. Use locking distribuído (`RedLock`) para prevenção de cache stampede em chaves de alto tráfego.

## Q83
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** Qual é a diferença entre métodos `abstract` e `virtual` em C#? Quando você usaria uma interface vs. uma classe abstrata?
**Ideal Answer:** `virtual`: fornece uma implementação padrão que classes derivadas podem sobrescrever. `abstract`: declara um método sem implementação; classes derivadas devem sobrescrevê-lo. Uma classe abstrata pode ter membros abstratos e concretos, não pode ser instanciada e pode manter estado. Interface: contrato puro sem estado (campos), suporta herança múltipla e, desde o C# 8, pode ter implementações padrão. Use uma classe abstrata quando: compartilhando implementação em uma hierarquia, precisa de uma base comum com implementação parcial ou precisa de membros protegidos. Use uma interface quando: definindo um contrato que múltiplos tipos não relacionados implementarão, ou habilitando injeção de dependência com múltiplas implementações.

## Q84
**Category:** Linguagem C#
**Difficulty:** Hard
**Question:** O que são expression trees em C#, e como o EF Core as usa para traduzir LINQ para SQL?
**Ideal Answer:** Uma expression tree representa código como dados — uma árvore de nós `Expression` que pode ser inspecionada e transformada em tempo de execução. Quando você escreve `context.Users.Where(u => u.Email == email)`, o lambda é capturado como `Expression<Func<User, bool>>` (não compilado para um delegate). O provedor LINQ do EF Core percorre essa expression tree e a traduz para SQL: `WHERE email = @email`. É por isso que o EF Core pode traduzir LINQ para SQL, mas não pode traduzir chamadas de métodos C# arbitrários (métodos não compreendidos pelo provedor lançam `InvalidOperationException`). Expression trees também são usadas em mapeamento ORM, construtores de consultas dinâmicas (padrão Specification) e source generators.

## Q85
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** O que são nullable reference types no C# 8+, e como ajudam a prevenir `NullReferenceException`?
**Ideal Answer:** Nullable reference types (NRT) introduzem análise de null em tempo de compilação. Quando habilitado (`<Nullable>enable</Nullable>`), tipos por referência são não-nullable por padrão — `string name` garante não-null, `string? name` permite null explicitamente. O compilador emite avisos quando: um valor nullable é usado sem verificação de null, ou quando um parâmetro/propriedade não-nullable pode receber null. Isso captura bugs de `NullReferenceException` em tempo de compilação em vez de em tempo de execução. Migre código habilitando NRT por arquivo primeiro, adicionando anotações `?` e verificações de null. NRT não muda comportamento em tempo de execução — é puramente uma ferramenta de análise em tempo de compilação.

## Q86
**Category:** Padrões de Design
**Difficulty:** Medium
**Question:** O que é o padrão Observer, e como é implementado no .NET? Dê um exemplo além de delegates `event`.
**Ideal Answer:** O padrão Observer define uma dependência um-para-muitos onde observadores são notificados de mudanças de estado. No .NET: (1) Delegates `event` — a forma mais simples; publicadores disparam eventos e assinantes registram handlers. (2) `IObservable<T>`/`IObserver<T>` — o modelo reativo push-based usado por Reactive Extensions (Rx.NET). (3) `INotifyPropertyChanged` — usado em data binding WPF/MAUI. (4) Notificações MediatR — `INotificationHandler<T>` para despacho de eventos de domínio in-process. (5) Channels (`System.Threading.Channels`) — uma fila produtor/consumidor para comunicação desacoplada dentro de um processo. Escolha baseado em se você precisa de notificação síncrona/assíncrona, backpressure ou fan-out.

## Q87
**Category:** Performance
**Difficulty:** Hard
**Question:** O que é `ArrayPool<T>` e `MemoryPool<T>`, e quando devem ser usados em vez de `new T[]`?
**Ideal Answer:** `ArrayPool<T>.Shared` fornece um pool de arrays reutilizáveis, evitando alocação repetida no heap e pressão do GC para buffers grandes temporários. Alugue com `ArrayPool<byte>.Shared.Rent(minLength)` e devolva com `ArrayPool<byte>.Shared.Return(buffer, clearArray: true)`. O array retornado pode ser maior que o solicitado — sempre rastreie o tamanho real usado. `MemoryPool<T>` é o equivalente abstrato, seguro para async, retornando `IMemoryOwner<T>` que implementa `IDisposable` para devolução segura via `using`. Casos de uso: leitura do corpo de requisição HTTP, parsing de protocolo binário, I/O de arquivo, qualquer código que aloca arrays de bytes de vida curta em caminhos frequentes. Nunca esqueça de devolver o array alugado — falha em devolver causa exaustão do pool.

## Q88
**Category:** Segurança
**Difficulty:** Hard
**Question:** O que é PKCE, e por que é obrigatório para fluxos de authorization code OAuth 2.0 em clientes nativos/SPA?
**Ideal Answer:** PKCE (Proof Key for Code Exchange) previne ataques de interceptação de authorization code em clientes públicos (apps nativos, SPAs) que não conseguem armazenar um client secret de forma segura. Fluxo: o cliente gera um `code_verifier` aleatório, computa `code_challenge = BASE64URL(SHA256(code_verifier))` e inclui o challenge na requisição de autorização. Ao trocar o código por um token, o cliente envia o `code_verifier` original. O servidor verifica que corresponde ao challenge. Um atacante que intercepte o authorization code não pode trocá-lo sem o `code_verifier` original. PKCE é obrigatório para clientes públicos segundo OAuth 2.1 e recomendado para todos os fluxos de authorization code independente do tipo de cliente.

## Q89
**Category:** Testes
**Difficulty:** Hard
**Question:** O que é teste de mutação, e como ele complementa métricas de cobertura de código?
**Ideal Answer:** Teste de mutação avalia a qualidade da suíte de testes introduzindo pequenos bugs intencionais (mutações) no código (por exemplo, mudando `>` para `>=`, invertendo um booleano) e verificando que os testes existentes os capturam (matam o mutante). Se uma mutação sobrevive (testes ainda passam com um bug no código), a suíte de testes tem uma lacuna. Cobertura de código apenas mede se uma linha foi executada, não se o teste verifica comportamento significativo. Um teste pode alcançar 100% de cobertura sem verificar nada. Teste de mutação fornece um sinal muito mais alto. Ferramentas: Stryker.NET para C#. Altas pontuações de mutação indicam que testes estão genuinamente verificando comportamento, não apenas exercitando caminhos de código.

## Q90
**Category:** Bancos de Dados
**Difficulty:** Hard
**Question:** Qual é a estratégia de migração de banco de dados para implantações com zero downtime? Como você lida com mudanças de esquema que quebram?
**Ideal Answer:** Migrações com zero downtime requerem compatibilidade retroativa entre as versões antiga e nova do código. Estratégia: (1) Expandir: adicione nova coluna como nullable (sem default necessário, código antigo a ignora, código novo escreve nela). (2) Migrar: preencha dados em lotes para evitar locks de tabela. (3) Contrair: após todas as instâncias executarem o novo código, adicione restrições ou remova colunas antigas. Nunca adicione uma coluna NOT NULL sem default em uma única implantação. Nunca renomeie colunas ou tabelas em um passo — adicione nova, migre, remova antiga em múltiplas implantações. Use feature flags para desacoplar implantação de ativação de funcionalidade. Migrações do EF Core devem ser aplicadas separadamente da implantação de código (por exemplo, como um init container no Kubernetes).

## Q91
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** O que é o padrão Saga para transações distribuídas? Compare coreografia vs. orquestração.
**Ideal Answer:** Uma Saga é uma sequência de transações locais coordenadas para alcançar uma transação distribuída sem two-phase commit. Cada passo tem uma transação compensatória para rollback. Coreografia: cada serviço escuta eventos e reage, emitindo mais eventos. Descentralizado, sem ponto único de falha, mas difícil de visualizar e depurar o fluxo geral. Orquestração: um coordenador central (orquestrador da saga) chama cada serviço em sequência e emite comandos compensatórios em caso de falha. Mais fácil de entender e monitorar, mas o orquestrador pode se tornar um gargalo. No .NET, Sagas do MassTransit implementam orquestração como máquinas de estado. Use sagas para: fulfillment de pedidos, fluxos de pagamento multi-etapa e qualquer processo que abrange múltiplos serviços.

## Q92
**Category:** Bancos de Dados
**Difficulty:** Medium
**Question:** Qual é a diferença entre Dapper e EF Core, e quando você usaria cada um no mesmo projeto?
**Ideal Answer:** EF Core: ORM completo com change tracking, migrações, tradução LINQ-para-SQL, gerenciamento de relacionamentos. Ideal para operações de escrita (comandos) onde você precisa de change tracking, controle de concorrência e gerenciamento de esquema. Dapper: micro-ORM — wrapper fino sobre ADO.NET que mapeia resultados de consultas SQL para objetos. Sem change tracking, sem tradução LINQ. Mais rápido e simples para operações de leitura. Padrão ideal em CQRS: command handlers usam EF Core para escritas (aproveitando change tracking e migrações), query handlers usam Dapper para leituras (SQL otimizado, projeções, JOINs, agregações). Compartilhe o mesmo `IDbConnection`/`DbContext` connection. Isso dá a corretude do EF Core para escritas e a performance de SQL puro para leituras.

## Q93
**Category:** Linguagem C#
**Difficulty:** Hard
**Question:** Qual é a diferença entre `Task.WhenAll` e `Task.WhenAny`? Quais são os riscos de usar `Task.WhenAll` sem tratamento de erro adequado?
**Ideal Answer:** `Task.WhenAll` completa quando todas as tasks terminam — se qualquer task falha, relança a primeira exceção; outras exceções são armazenadas em `AggregateException.InnerExceptions`. `Task.WhenAny` completa assim que a primeira task completa (ou falha). Risco com `WhenAll`: se você usa `await` sem try/catch, apenas a primeira exceção é exibida; as outras são silenciosamente ignoradas, potencialmente escondendo falhas. Para inspecionar todas as exceções: capture `AggregateException` ou verifique cada task individualmente após `WhenAll`. Outro risco: se tasks não são limitadas, lançar milhares de tasks simultaneamente pode esgotar o ThreadPool ou pool de conexões. Use `SemaphoreSlim` para limitar concorrência ao processar coleções grandes.

## Q94
**Category:** Azure
**Difficulty:** Medium
**Question:** O que é Azure Cosmos DB, e quais vantagens e desvantagens ele tem comparado ao Azure SQL Database para um backend .NET?
**Ideal Answer:** Cosmos DB é um banco de dados NoSQL globalmente distribuído, multi-modelo, com latência garantida de um dígito de milissegundo e níveis de consistência configuráveis (forte a eventual). Pontos fortes: scale-out horizontal, geo-replicação, flexibilidade de esquema, múltiplas APIs (SQL, Mongo, Cassandra). Desvantagens vs. Azure SQL: sem transações ACID entre partições (limitado a partição única ou multi-documento dentro de uma partição em versões mais novas), sem JOINs arbitrários, design de esquema deve se alinhar com padrões de acesso, e modelo de cobrança por RU (Request Unit) pode ser caro e difícil de prever. Use Cosmos DB para: workloads de alto volume, globalmente distribuídos, com padrões de acesso bem definidos. Use Azure SQL para: dados relacionais, consultas complexas, relatórios e quando garantias ACID são críticas.

## Q95
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** O que é o padrão strangler fig, e como você o aplicaria para decompor um monolito incrementalmente?
**Ideal Answer:** Nomeado após uma árvore que lentamente envolve seu hospedeiro, o padrão Strangler Fig substitui incrementalmente um sistema legado construindo nova funcionalidade em um novo sistema ao lado do antigo, redirecionando tráfego fatia por fatia. Passos: (1) Identifique uma fatia (uma funcionalidade ou limite de domínio) para extrair. (2) Construa o novo serviço implementando aquela fatia. (3) Use uma fachada ou proxy reverso (YARP, NGINX, API Gateway) para rotear tráfego daquela fatia para o novo serviço. (4) Verifique corretude, então descomissione o código antigo. (5) Repita. Benefícios: sem reescrita big-bang, entrega contínua, reversível em cada passo. Desafio principal: compartilhamento de banco durante a transição — use banco compartilhado inicialmente, extraia propriedade de esquema ao longo do tempo com camadas de tradução ACL.

## Q96
**Category:** Linguagem C#
**Difficulty:** Medium
**Question:** O que são setters somente `init` no C# 9, e como suportam construção de objetos imutáveis?
**Ideal Answer:** Setters `init` só podem ser chamados durante a inicialização do objeto (no construtor ou em um inicializador de objeto), não depois. Eles permitem: `var obj = new MyClass { Name = "test" }` (sintaxe legível) enquanto previnem mutação após construção (`obj.Name = "other"` é um erro de compilação). Diferente de campos `readonly`, propriedades `init` podem ser usadas em inicializadores de objeto e expressões `with` para records. São ideais para DTOs e value objects que devem ser imutáveis após construção enquanto ainda suportam sintaxe de inicialização legível. Combinados com o modificador `required` (C# 11), eles impõem que a propriedade seja definida no momento da inicialização.

## Q97
**Category:** Padrões de Design
**Difficulty:** Hard
**Question:** O que é o padrão Strategy, e como se compara ao uso de polimorfismo (herança) no .NET?
**Ideal Answer:** O padrão Strategy define uma família de algoritmos, encapsula cada um e os torna intercambiáveis em tempo de execução via composição. No .NET: injete `IPaymentStrategy` em um handler; em tempo de execução injete `StripePaymentStrategy` ou `PayPalPaymentStrategy`. Com herança (polimorfismo), o algoritmo é embutido na hierarquia de classes — trocar comportamento requer uma subclasse diferente, e múltiplos comportamentos requerem múltiplos níveis de herança. Strategy usa composição: o comportamento é um objeto separado, injetável. Benefícios: mais fácil adicionar novas strategies sem modificar classes existentes (aberto/fechado), mais fácil testar cada strategy isoladamente e suporta seleção em tempo de execução. Prefira composição sobre herança para variação de comportamento.

## Q98
**Category:** Runtime .NET
**Difficulty:** Hard
**Question:** Qual é a diferença entre `Thread`, `Task`, `ThreadPool` e `async/await` na concorrência .NET?
**Ideal Answer:** `Thread`: uma thread no nível do SO. Custosa (1MB de pilha por padrão). Use apenas para trabalho bloqueante de longa execução. `ThreadPool`: um pool de threads reutilizáveis gerenciado pelo CLR. Tasks são enfileiradas nele. Expandir o pool é lento. `Task`: representa uma operação assíncrona — pode executar em uma thread do ThreadPool ou completar sem uma thread (I/O puro). `async/await`: açúcar sintático sobre `Task` que gera uma máquina de estado; o método suspende em cada `await`, liberando a thread para outro trabalho, e retoma quando a operação aguardada completa. Para trabalho I/O-bound (banco de dados, HTTP), `async/await` é ideal — nenhuma thread é bloqueada enquanto aguarda. Para trabalho CPU-bound, use `Task.Run` para delegar ao ThreadPool sem bloquear a thread da requisição.

## Q99
**Category:** Design de API
**Difficulty:** Medium
**Question:** O que é FluentValidation, e como você o integra com uma API ASP.NET Core para validação de requisições?
**Ideal Answer:** FluentValidation fornece uma API fluente para construir regras de validação em classes validadoras dedicadas, separando lógica de validação do código de domínio e controller. Defina `class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>` com regras como `RuleFor(x => x.Email).NotEmpty().EmailAddress()`. Registre com `services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>()`. Em minimal APIs ou MediatR, valide manualmente: `var result = validator.Validate(command); if (!result.IsValid) return Results.ValidationProblem(result.ToDictionary())`. Para controllers, use a integração `FluentValidation.AspNetCore` para conectar ao `ModelState` automaticamente. Comparado a Data Annotations, FluentValidation é mais expressivo, suporta regras condicionais, validação entre propriedades e é mais fácil de testar unitariamente.

## Q100
**Category:** Arquitetura
**Difficulty:** Hard
**Question:** Como você projeta para observabilidade desde o primeiro dia em um microsserviço .NET? Qual é a diferença entre monitoramento e observabilidade?
**Ideal Answer:** Monitoramento responde "o sistema está de pé?" com métricas e alertas predefinidos. Observabilidade responde "por que o sistema está se comportando assim?" explorando modos de falha desconhecidos através de telemetria (logs, métricas, traces). Projete para observabilidade: (1) Logging estruturado com IDs de correlação e contexto de requisição (Serilog + enrichers). (2) Rastreamento distribuído com OpenTelemetry — auto-instrumente HTTP, banco e mensageria; propague contexto de trace. (3) Métricas: taxa de requisições, taxa de erros, percentis de latência (p50, p95, p99), profundidade de fila — exponha via endpoint Prometheus ou OTLP. (4) Health checks com verificações significativas (não apenas "vivo"). (5) Alertas em violações de SLO, não apenas saúde do servidor. Observabilidade é uma preocupação de engenharia de primeira classe, não uma reflexão tardia.
