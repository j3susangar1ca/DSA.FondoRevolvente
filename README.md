# Especificación Funcional y de Interfaz — Edición Práctica
## Sistema de Gestión de Fondo Revolvente · Hospital Civil de Guadalajara
### Documento de Diseño UX/UI · Stack Tecnológico Realista

---

| Atributo | Valor |
|---|---|
| **Identificador del Documento** | FR-UX-SPEC-2026-003-R1 |
| **Plataforma Objetivo** | Windows 11 (Build 22621 / 22H2 y posteriores) |
| **Plataforma Desarrollo** | Linux Fedora KDE Plasma 43 |
| **Framework de Presentación** | Uno Platform — WinUI 3 Flavor · Windows App SDK 1.6+ (o web app equivalente) |
| **Patrón de UI** | MVVM (Model-View-ViewModel) · CommunityToolkit.Mvvm |
| **Sistema de Diseño** | Microsoft Fluent Design System v2 · WinUI 3 (o equivalente en framework destino) |
| **Ciclo de Vida del Proceso** | **7 estados core · ~10 transiciones · 5 reglas de negocio** |
| **Roles Documentados** | Administrador · Comprador DSA · Miembro CAA · Recursos Financieros · Almacén · Consulta DSA |
| **Estado del Documento** | Aprobado para Diseño e Implementación |

---

## Tabla de Contenidos

| Sección | Título |
|---|---|
| **1** | Introducción y Alcance del Documento |
| **2** | Arquitectura de Presentación — Stack Tecnológico Simplificado |
| **3** | Modelo de Roles, Permisos y Seguridad de Interfaz |
| **4** | Sistema Global de Diseño (Design System) |
| **5** | Módulo 01 — Pantalla de Autenticación |
| **6** | Módulo 02 — Shell Principal de la Aplicación |
| **7** | Módulo 03 — Dashboard de Estado General |
| **8** | Módulo 04 — Gestión de Solicitudes: Vista de Lista |
| **9** | Módulo 05 — Nueva Solicitud: Formulario de Creación |
| **10** | Módulo 06 — Detalle de Solicitud: Expediente Completo |
| **11** | Módulo 07 — Historial de Acciones del Expediente |
| **12** | Módulo 08 — Gestión de Proveedores |
| **13** | Módulo 09 — Cotizaciones y Cuadro Comparativo |
| **14** | Módulo 10 — Validación Fiscal: Panel CFDI |
| **15** | Módulo 11 — Reportes y Exportación |
| **16** | Módulo 12 — Panel de Administración: Roles y Accesos |
| **17** | Especificación Transversal: Ciclo de Vida de la Solicitud (7 estados) |
| **Apéndice A** | Sistema de Colores por Estado |
| **Apéndice B** | Matriz de Visibilidad de Módulos por Rol |
| **Apéndice C** | Representación Visual de Reglas de Negocio |
| **Apéndice D** | Glosario de Términos |

---

## 1. Introducción y Alcance del Documento

### 1.1 Propósito

Este documento constituye la **Especificación Funcional y de Interfaz** del Sistema de Gestión de Fondo Revolvente del Hospital Civil de Guadalajara (HCG). Su objetivo es definir de manera precisa y tecnológicamente realista la experiencia de usuario (UX), la interfaz de usuario (UI), los flujos de interacción, los comportamientos visuales y los criterios de aceptación de cada módulo del sistema.

El documento parte de la especificación FR-UX-SPEC-2026-003 y la ajusta eliminando complejidad arquitectónica y de proceso que excede las necesidades reales del sistema.

### 1.2 Alcance Funcional

El sistema gestiona el ciclo de vida completo de las adquisiciones realizadas mediante el mecanismo de **Fondo Revolvente** del HCG, con un límite normativo de **$75,000 MXN** por operación (Ley de Compras del Estado de Jalisco, Art. 57 del Reglamento). Comprende:

- **7 estados** en el ciclo de vida de cada solicitud de adquisición.
- **~10 transiciones** de estado gobernadas por lógica de negocio en la capa de servicio.
- **5 reglas de negocio críticas** (RN-001 a RN-005) con validación en API y UI.
- **6 roles funcionales** con permisos diferenciados sobre cada módulo y acción.
- Integración con el **SAT** para validación CFDI 4.0 (con manejo de error simple y botón manual de reintento).
- Integración con el **directorio activo (Active Directory)** del hospital para autenticación y autorización.
- Acceso al **repositorio de archivos SMB** del hospital para gestión documental.
- Generación de **reportes** en PDF y Excel mediante SSRS.

### 1.3 Audiencia del Documento

| Audiencia | Uso Principal |
|---|---|
| **Equipo de Diseño UX/UI** | Referencia de composición visual, componentes y flujos de interacción |
| **Equipo de Desarrollo Frontend** | Especificación de componentes, comportamientos y contratos de interfaz |
| **Equipo de Desarrollo Backend** | Comprensión de los requerimientos de datos, estados y respuestas API |
| **Product Manager / Owner** | Validación funcional de flujos, reglas de negocio y criterios de aceptación |
| **Comité de Auditoría y Cumplimiento** | Verificación del cumplimiento normativo (límites, trazabilidad, roles) |
| **Equipo de QA** | Base para la construcción de casos de prueba |

### 1.4 Restricciones del Documento

- Este documento **no contiene** fragmentos de código fuente en ningún lenguaje.
- Los nombres de controles de interfaz se expresan de forma **genérica** (ej: "campo de búsqueda con autocompletado", "modal de confirmación", "indicador numérico") para no atar la implementación a un framework específico. Las notas de implementación al pie de cada módulo ofrecen la equivalencia en WinUI 3 como referencia.
- Las especificaciones de comportamiento visual son **normativas** en cuanto a función (qué debe comunicar el elemento), pero no en cuanto a parámetros de animación (duración en milisegundos, curvas de interpolación). Estos los gestiona el framework de UI por defecto.

---

## 2. Arquitectura de Presentación — Stack Tecnológico Simplificado

### 2.1 Modelo de Capas de la Aplicación

La arquitectura se organiza en tres capas con responsabilidades claras. La complejidad se mantiene en el mínimo necesario para el volumen de operaciones de un hospital regional.

```
┌─────────────────────────────────────────────────────────────────────┐
│  CAPA DE PRESENTACIÓN — Uno Platform (WinUI 3) / Web App            │
│  ─────────────────────────────────────────────────────────────────  │
│  Vista: Composición visual · Fluent Design                          │
│  ViewModel (MVVM): Estado de la UI · Comandos                       │
│  Polling cada 30s para refrescar datos cuando la app está abierta   │
└─────────────────────────────────────────────────────────────────────┘
                       ▼ HTTPS (Red interna hospitalaria)
┌─────────────────────────────────────────────────────────────────────┐
│  CAPA DE API — ASP.NET Core Web API (.NET 8)                        │
│  ─────────────────────────────────────────────────────────────────  │
│  Controladores CRUD estándar · JWT Auth (Active Directory)          │
│  Validación de reglas de negocio (RN-001 a RN-005)                  │
│  Try/catch para integración SAT con mensaje de error amigable       │
└─────────────────────────────────────────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────────────────┐
│  CAPA DE DATOS                                                       │
│  ─────────────────────────────────────────────────────────────────  │
│  Entity Framework Core (lecturas y escrituras)                      │
│  SQL Server Express 2022 · SMB 3.0 (repositorio de expedientes PDF) │
│  SSRS (generación de reportes PDF/Excel)                            │
└─────────────────────────────────────────────────────────────────────┘
```

### 2.2 Implicaciones de Diseño del Patrón MVVM

El patrón **MVVM** con `CommunityToolkit.Mvvm` mantiene su relevancia independientemente de las simplificaciones backend:

**Reactividad de estados:** Cada pantalla que visualiza el estado de una solicitud no requiere recarga manual. El ViewModel expone el estado como propiedad observable; cuando cambia (por acción del usuario o por el ciclo de polling), todos los componentes visuales ligados se actualizan automáticamente.

**Comandos de UI:** Cada botón de acción principal (Autorizar, Rechazar, Validar CFDI, Confirmar Entrega) está ligado a un Comando del ViewModel. El estado de habilitación del botón se calcula reactivamente en función del estado actual de la solicitud y del rol del usuario autenticado.

**Actualización por polling:** El ViewModel implementa un temporizador simple que consulta la API cada 30 segundos para refrescar los datos de la pantalla activa. Si la app está en segundo plano, el polling se pausa. Si el usuario prefiere actualizar manualmente, dispone de un botón "Actualizar" en la barra de comandos de cada módulo.

### 2.3 Bloqueo de Edición Simplificado (RN-005)

La probabilidad de que dos usuarios editen la misma solicitud al mismo milisegundo es extremadamente baja en un hospital regional. El mecanismo de bloqueo se implementa como un **flag simple en la base de datos**:

- Campo `BloqueadoPor` (UsuarioId, nullable) + campo `BloqueadoDesde` (DateTime, nullable) en la tabla de Solicitudes.
- Cuando un usuario abre un expediente en modo edición, la API actualiza estos campos.
- El bloqueo expira automáticamente si `BloqueadoDesde` tiene más de 30 minutos (verificado en el endpoint de apertura).
- Al guardar o abandonar la pantalla, la API limpia los campos.
- Si otro usuario intenta editar un expediente bloqueado, la API retorna un error descriptivo y la UI muestra un modal: "Este expediente está siendo editado por [Nombre] desde las [HH:MM]. Puede verlo en modo de solo lectura."

### 2.4 Estructura del Proyecto (Simplificada)

```
HCG.FondoRevolvente/
├── HCG.FondoRevolvente.sln
├── src/
│   ├── HCG.FondoRevolvente.Domain/
│   │   ├── Entities/          (Solicitud, Proveedor, Cotizacion)
│   │   ├── Enums/             (EstadoSolicitud, RolAplicacion)
│   │   ├── Constants/         (LimitesNegocio)
│   │   └── Exceptions/        (DomainException y subclases)
│   │
│   ├── HCG.FondoRevolvente.Application/
│   │   ├── Services/          (SolicitudService, CfdiService, ReporteService)
│   │   ├── DTOs/              (SolicitudDto, CotizacionDto, etc.)
│   │   └── Interfaces/        (ISolicitudRepository, ICfdiValidator)
│   │
│   ├── HCG.FondoRevolvente.Infrastructure/
│   │   ├── Data/              (AppDbContext, repositorios EF Core)
│   │   ├── ExternalServices/  (SatCfdiValidator, SmbFileService, SsrsReportService)
│   │   └── Identity/          (ActiveDirectoryAuthService)
│   │
│   ├── HCG.FondoRevolvente.Api/
│   │   ├── Controllers/       (SolicitudesController, CotizacionesController, etc.)
│   │   └── Middleware/        (ErrorHandlingMiddleware, JwtMiddleware)
│   │
│   └── HCG.FondoRevolvente.Client/
│       ├── ViewModels/        (uno por módulo)
│       ├── Views/             (uno por módulo)
│       ├── Services/          (ApiClientService, AuthService, PollingService)
│       └── Components/        (StateBadge, MontoDisplay, BloqueoIndicator)
│
└── tests/
    ├── Domain.Tests/
    ├── Application.Tests/
    └── Integration.Tests/
```

---

## 3. Modelo de Roles, Permisos y Seguridad de Interfaz

### 3.1 Roles de la Aplicación

La autenticación se realiza contra el **Active Directory** del hospital. Los grupos de AD determinan el JWT que recibe el cliente. La interfaz aplica **seguridad de presentación**: los elementos a los que un rol no tiene acceso no se deshabilitan — directamente no se renderizan.

| Grupo Active Directory | Rol en la Aplicación | Descripción Funcional |
|---|---|---|
| `Administradores_Sistema` | **Administrador** | Acceso completo a todos los módulos, acciones y datos. |
| `Compradores_DSA` | **Comprador DSA** | Creación y gestión de solicitudes propias. Cotizaciones, proveedores. |
| `CAA_Miembros` | **Revisor CAA** | Consulta de expedientes en estado de revisión CAA. Autorización y rechazo. |
| `Recursos_Financieros` | **Finanzas** | Validación fiscal (CFDI) y registro de pago. |
| `Almacen_Staff` | **Almacén** | Confirmación de recepción de bienes. |
| `DSA_Staff` | **Consulta DSA** | Acceso de solo lectura a solicitudes, reportes y dashboard. |

### 3.2 Principio de Mínimo Privilegio en la Interfaz

La interfaz implementa mínimo privilegio visual: cada usuario ve únicamente lo que necesita para ejecutar su función.

- El menú de navegación solo muestra los ítems relevantes al rol autenticado.
- Los botones de acción en el panel de Detalle de Solicitud se renderizan únicamente si el rol actual tiene permiso **y** el estado actual de la solicitud permite esa transición.
- Los datos financieros (RFC de proveedores, montos detallados) son visibles solo para los roles con acceso.
- El filtrado de la lista de solicitudes aplica automáticamente el scope del rol.

### 3.3 Gestión Visual del Bloqueo de Edición (RN-005)

Al intentar editar un expediente bloqueado por otro usuario, la UI muestra un modal informativo (ver sección 2.3). Si el usuario actual tiene el bloqueo activo, un **indicador de bloqueo** prominente aparece en el encabezado del expediente mostrando "Editando ahora" con opción de "Liberar bloqueo". El bloqueo se renueva automáticamente en segundo plano cada 25 minutos.

---

## 4. Sistema Global de Diseño (Design System)

### 4.1 Lenguaje Visual

La experiencia visual se fundamenta en el **Microsoft Fluent Design System v2** para Windows 11, con la posibilidad de trasladarse a un sistema de diseño equivalente si la plataforma cambia a web.

**Materiales y Profundidad:** Se usan tres materiales principales: el fondo base de las ventanas (Mica), los paneles superpuestos como menús desplegables y paneles laterales (Acrylic), y el oscurecimiento de fondo cuando hay un modal activo (Smoke/overlay semitransparente).

**Tipografía:** Jerarquía estándar con la fuente de sistema del OS (Segoe UI Variable en Windows 11): Display para KPIs prominentes, Title para encabezados de módulo, Subtitle para secciones, Body para contenido principal y Caption para metadatos secundarios.

**Iconografía:** Segoe Fluent Icons como familia principal.

### 4.2 Paleta de Colores del Sistema

| Token de Color | Hex | Uso |
|---|---|---|
| `Success` | `#6CCB5F` | Confirmaciones, validaciones exitosas |
| `Critical` | `#C50F1F` | Estados de error, rechazos, validaciones fallidas |
| `Caution` | `#F7630C` | Advertencias, alertas de proceso |
| `Attention` | `#005FB7` | Información relevante |
| `Neutral` | `#69797E` | Estados terminales, elementos inactivos |

### 4.3 Componentes de Diseño Transversales

**StateBadge (Pastilla de Estado):** Componente reutilizable que representa el estado actual de una solicitud con color, ícono y texto. Disponible en variantes Small (listas de alta densidad), Medium (estándar) y Large (encabezados).

**BloqueoIndicator:** Barra de advertencia contextual visible en el encabezado de un expediente cuando tiene un bloqueo de edición activo. Muestra quién está editando y desde qué hora.

**MontoDisplay:** Componente de visualización monetaria que formatea valores como `$XX,XXX.XX MXN` e incluye una barra de progreso respecto al límite de $75,000 MXN, con colores semafóricos (verde < 70%, amarillo 70-90%, rojo > 90%).

---

## 🖥️ Módulo 01: Pantalla de Autenticación

### Propósito Funcional

Punto de entrada único al sistema. Valida credenciales institucionales contra el **Active Directory del HCG**, construye el contexto de permisos de la sesión y redirige al módulo de destino del rol. El sistema aplica bloqueo progresivo de cuenta tras intentos fallidos repetidos.

### Composición Visual

Ventana compacta y fija centrada sobre el escritorio. Organizada verticalmente en: zona de identidad institucional (logotipo + título del sistema), zona de credenciales (campos de usuario y contraseña), enlace de soporte técnico, botón de acción principal y barra de estado contextual para mensajes de error.

### Elementos de Interfaz

| Componente | Descripción |
|---|---|
| **Campo de Usuario** | Etiqueta "Usuario de red" · Placeholder `dominio\usuario` · Autocompletar dominio institucional. |
| **Campo de Contraseña** | Botón de revelación integrado. Tecla Enter activa la validación. |
| **Enlace de Soporte** | "¿Problemas para ingresar?" → abre cliente de correo con dirección de TI preconfigurada. |
| **Botón Principal** | "Iniciar sesión" · Ancho completo · Muestra indicador de carga (spinner) durante la validación. |
| **Barra de Estado** | Colapsada por defecto. Emerge cuando hay mensajes de error, advertencia o información. |

### Comportamientos Clave

**Validación:** Los campos validan al intentar enviar (no durante la escritura). En error de credenciales, la barra de estado muestra el mensaje de error ("Credenciales incorrectas. Intento 2 de 3.") y el campo de contraseña se limpia para reingreso.

**Cuenta bloqueada (3 intentos):** Los campos y el botón se deshabilitan. El mensaje indica contactar a TI.

**Error de conectividad:** La barra de estado indica que no se puede conectar con el directorio del hospital e incluye un botón "Reintentar".

**Ingreso exitoso:** La sesión se establece con el JWT. La aplicación navega al módulo de destino del rol activo: Dashboard para Admin/Consulta, Lista de Solicitudes para roles operativos.

---

## 🖥️ Módulo 02: Shell Principal de la Aplicación

### Propósito Funcional

Marco estructural persistente que orquesta la navegación entre módulos durante la sesión activa. Provee identidad de marca, contexto del usuario autenticado, navegación principal, acceso a notificaciones básicas y el área de contenido donde se renderizan los módulos.

### Composición Visual

Panel de navegación lateral con el logotipo institucional en el encabezado, lista de ítems de navegación filtrada por rol, y en el pie: avatar del usuario con menú contextual (nombre, rol activo, botón de cerrar sesión).

La zona de contenido principal ocupa el área restante. Fondo Mica.

### Elementos de Interfaz

| Elemento | Descripción |
|---|---|
| **Menú de navegación** | Solo ítems accesibles al rol activo. Colapsa a iconos en ventanas estrechas. |
| **Avatar del usuario** | Iniciales del nombre en círculo de color de acento. Menú: nombre completo, rol, "Cerrar sesión". |
| **Indicador de actualización** | Texto "Última actualización: HH:MM" y botón de refresco manual visible en cada módulo. |
| **Área de contenido** | `ContentFrame` donde se renderiza el módulo activo. |

### Flujo Operativo

1. Al iniciar sesión, el Shell carga con el módulo de destino del rol.
2. El usuario navega entre módulos usando el menú lateral.
3. El polling de 30 segundos refresca automáticamente los datos del módulo activo.
4. Al cerrar sesión: modal de confirmación → JWT descartado → regreso a la pantalla de autenticación.

---

## 🖥️ Módulo 03: Dashboard de Estado General

### Propósito Funcional

Vista consolidada del estado operativo del sistema de Fondo Revolvente. Proporciona al Administrador, Comprador DSA y rol de Consulta una vista de: métricas KPI, alertas activas y una lista paginada de solicitudes recientes. Los datos se actualizan automáticamente cada 30 segundos mediante polling, o manualmente con el botón "Actualizar".

### Composición Visual

Tres zonas en scroll vertical: barra de control con selector de ejercicio fiscal y botón de actualización, cuadrícula de tarjetas KPI y lista de solicitudes recientes con paginación.

### Elementos de Interfaz

**Barra de Control:**

| Elemento | Descripción |
|---|---|
| **Selector "Ejercicio Fiscal"** | Lista desplegable con años disponibles. Al cambiar, recarga las métricas. |
| **Botón "Actualizar"** | Refresca todos los datos del dashboard manualmente. |
| **Texto "Última actualización"** | Timestamp de la última sincronización de datos. |

**Tarjetas KPI:**

| Tarjeta | Contenido |
|---|---|
| **Solicitudes Activas** | Total de solicitudes en estados no terminales. Clic filtra la lista por "En proceso". |
| **Monto Total Ejercido** | Suma acumulada del período en formato `$XX,XXX.XX MXN`. |
| **Tiempo Promedio de Ciclo** | Días promedio de resolución completa. Indicador de semáforo. |
| **Solicitudes en Alerta** | Solicitudes detenidas más de N días. Si > 0, la tarjeta se resalta en naranja. |

**Lista de Solicitudes Recientes:**

Tabla simple paginada (25 filas por página). Columnas: Folio DSA, Servicio Solicitante, Monto, Estado (StateBadge), Responsable, Días en Estado. La fila es clickeable y navega al Detalle.

**Alertas Activas:**

Panel colapsable al pie del Dashboard con lista de alertas organizadas por categoría: solicitudes con posible fraccionamiento (RN-002), solicitudes en error de validación SAT, solicitudes próximas a vencer plazos.

---

## 🖥️ Módulo 04: Gestión de Solicitudes — Vista de Lista

### Propósito Funcional

Listado filtrable de todas las solicitudes del scope del usuario. Punto de entrada principal para acceder a un expediente específico, crear nuevas solicitudes y ejecutar acciones rápidas. El scope se aplica automáticamente por rol.

### Composición Visual

Barra de comandos superior con filtros y botón de nueva solicitud, seguida de una tabla paginada de solicitudes.

### Elementos de Interfaz

**Barra de Comandos:**

| Elemento | Descripción |
|---|---|
| **Campo de búsqueda con autocompletado** | Busca por folio DSA, nombre del servicio o proveedor. Resultados al escribir (mínimo 2 caracteres). |
| **Filtro de Estado** | Lista desplegable con los 7 estados disponibles + opción "Todos". |
| **Filtro de Período** | Selector de rango de fechas. |
| **Botón "Actualizar"** | Refresca la lista. |
| **Botón "+ Nueva Solicitud"** | Visible solo para roles con permiso de creación. |

**Tabla de Solicitudes:**

Columnas: Folio DSA, Concepto de Compra, Servicio Solicitante, Monto, Estado (StateBadge), Responsable, Fecha de Creación, Días en Estado. Paginación simple (25 filas/página). Clic en fila navega al Detalle.

---

## 🖥️ Módulo 05: Nueva Solicitud — Formulario de Creación

### Propósito Funcional

Formulario estructurado para que el Comprador DSA registre una nueva solicitud de adquisición. Valida en tiempo real las reglas de negocio RN-001 (monto máximo) y RN-002 (fraccionamiento) antes de permitir el envío.

### Composición Visual

Formulario de una columna con secciones numeradas: datos del solicitante, concepto y justificación de la compra, partidas de la compra (tabla editable con totales) y panel de validación de reglas.

### Elementos de Interfaz

**Sección 1 — Datos del Solicitante:**

| Campo | Tipo | Descripción |
|---|---|---|
| Servicio/Área Solicitante | Campo de texto | Nombre del área que realiza la solicitud. |
| Nombre del Responsable | Campo de texto | Autocompletado con el usuario autenticado, editable. |
| Fecha Requerida | Selector de fecha | Fecha en que se necesita el bien o servicio. |

**Sección 2 — Concepto de la Compra:**

| Campo | Tipo | Descripción |
|---|---|---|
| Concepto General | Campo de texto | Descripción general del bien o servicio a adquirir. |
| Justificación | Campo de texto multilínea | Justificación de la necesidad de la adquisición. |

**Sección 3 — Partidas:**

Tabla editable con columnas: Código de Producto, Descripción, Cantidad, Precio Unitario, Subtotal (calculado). Botón "+ Agregar Partida" al pie. El componente **MontoDisplay** muestra el total acumulado con barra de progreso respecto al límite.

**Sección 4 — Alertas de Validación (condicional):**

Aparece automáticamente si el sistema detecta condiciones que activan RN-001 o RN-002. Ver Apéndice C para detalle de representación visual.

**Barra de Acciones (fija al pie):**

| Botón | Comportamiento |
|---|---|
| **"Registrar Solicitud"** | Valida todos los campos y ejecuta el registro. Deshabilitado si hay errores activos. |
| **"Guardar como Borrador"** | Guarda en estado `BORRADOR` sin iniciar el ciclo oficial. |
| **"Cancelar"** | Modal de confirmación ("¿Descarta los cambios no guardados?") y regresa a la lista. |

### Comportamientos Clave

**Validación progresiva:** Los campos validan al perder el foco, no durante la escritura. Los campos con error muestran borde en color Critical y un tooltip con el mensaje específico.

**Resultado del registro:** La solicitud se crea con folio único `DSA-YYYY-NNN` asignado automáticamente. El estado inicial es `EN_REVISION_CAA` (saltando `BORRADOR` si se eligió "Registrar"). La pantalla regresa a la Lista de Solicitudes, donde la nueva solicitud aparece al inicio. Un aviso de éxito confirma el registro.

*Nota de implementación WinUI 3: Los campos corresponden a `TextBox`, `DatePicker`, `DataGrid` editable. La barra de acciones es un `CommandBar` fijo.*

---

## 🖥️ Módulo 06: Detalle de Solicitud — Expediente Completo

### Propósito Funcional

Vista maestra y punto de acción central del sistema. Consolida en una sola pantalla toda la información del expediente: datos del formulario original, estado actual, historial de transiciones, cotizaciones asociadas, documentos adjuntos y las **acciones disponibles** para el usuario según su rol y el estado actual.

### Composición Visual

Composición de dos columnas en resoluciones ≥ 1280px y una sola columna en resoluciones menores:

- **Columna principal** (izquierda, 65%): Área con scroll que contiene las secciones del expediente como tarjetas apiladas.
- **Panel lateral de acciones** (derecha, 35%): Fijo durante el scroll. Muestra el estado actual, las acciones disponibles, el indicador de bloqueo y los enlaces a documentos.

Encabezado fijo por encima del scroll con: folio DSA, StateBadge del estado actual, metadatos básicos e indicador de bloqueo cuando aplique.

### Elementos de Interfaz

**Encabezado Fijo:**

| Elemento | Descripción |
|---|---|
| **Ruta de navegación (breadcrumb)** | "Solicitudes > DSA-2026-089" con navegación de retorno. |
| **Folio** | "DSA-2026-089" · Tipografía Display. |
| **StateBadge Large** | Estado actual con color, ícono y texto (variante grande). |
| **Metadatos** | "Servicio de Urgencias · Recibida el 15/01/2026 · Atiende: Juan Pérez" |
| **BloqueoIndicator** | Visible solo cuando hay un bloqueo activo. Muestra quién edita y desde qué hora. |

**Panel Lateral de Acciones:**

1. **Estado actual:** StateBadge Medium + nombre de la fase + días en el estado.
2. **Acciones disponibles:** Solo los botones ejecutables para `{estado_actual, rol_usuario}`. No se muestran botones deshabilitados.

| Estado | Rol | Acciones |
|---|---|---|
| `EN_REVISION_CAA` | CAA | "Autorizar Solicitud" · "Rechazar Solicitud" |
| `CFDI_VALIDADO` | Finanzas | "Registrar Pago" |
| `PROVEEDOR_SELECCIONADO` | Almacén | "Confirmar Recepción" · "Reportar Discrepancia" |
| `EN_REVISION_CAA` | Comprador | (solo lectura) |
| `BORRADOR` | Comprador | "Editar Solicitud" · "Enviar a Revisión CAA" |

3. **Documentos:** Enlace "Ver documentos del expediente" (abre ruta SMB). Enlace "Ver Cuadro Comparativo" (si hay cotizaciones).

**Sección "Datos del Expediente":** Todos los campos del formulario original en modo solo lectura. Botón "Editar Datos" visible solo si el rol y el estado lo permiten. Al presionar, intenta adquirir el bloqueo.

**Sección "Cotizaciones Registradas":** Tabla con columnas: Proveedor, RFC (parcialmente enmascarado), Monto, Días de Entrega, Condiciones, PDF adjunto, Seleccionada. Botón "+ Agregar Cotización" visible según rol y estado.

**Sección "Historial de Acciones":** Lista cronológica inversa de todas las transiciones y acciones del expediente. Cada ítem: timestamp, estado anterior, estado nuevo, usuario, observaciones. Ver Módulo 07 para la vista completa.

### Comportamientos Clave

**Refresco de datos:** Al volver a la pantalla o al presionar "Actualizar", los datos del expediente se recargan desde la API.

**Ejecución de una acción (ej: Autorizar):**
1. El miembro CAA presiona "Autorizar Solicitud".
2. Emerge un modal de confirmación: título, descripción de la consecuencia, campo de observaciones opcionales, botones "Confirmar" y "Cancelar".
3. Al confirmar: el modal cierra, el StateBadge se actualiza al nuevo estado, el panel de acciones actualiza sus botones y un aviso de éxito confirma la transición.

*Nota de implementación WinUI 3: Las acciones se presentan como `Button` con `AccentButtonStyle` o estilos semánticos de color. El modal es un `ContentDialog`. Las secciones son `CardPanel` con `Shadow Depth 4`.*

---

## 🖥️ Módulo 07: Historial de Acciones del Expediente

### Propósito Funcional

Vista detallada del historial completo de acciones, transiciones de estado y observaciones de un expediente. Reemplaza la vista de "27 hitos en 8 fases" con un **log de transiciones de estado** simple y un log de acciones secundarias, que es todo lo que se necesita para trazabilidad y auditoría.

### Composición Visual

El módulo muestra dos secciones en una sola columna:

1. **Barra de progreso de estado** (parte superior): Representación visual lineal horizontal de los 7 estados del ciclo de vida, con indicación del estado actual y los completados.
2. **Historial cronológico** (parte inferior, en scroll): Lista de todas las entradas de historial ordenadas de más reciente a más antigua.

### Elementos de Interfaz

**Barra de Progreso de Estado:**

Línea de tiempo horizontal con 7 nodos (uno por estado del ciclo de vida). Estado completado: ícono ✅ en color del estado. Estado activo: ícono de reloj animado, resaltado con el color del estado. Estado futuro: ícono neutro gris.

**Lista de Historial:**

| Columna | Descripción |
|---|---|
| Timestamp | Fecha y hora completa `dd/MM/YYYY HH:MM:SS` |
| Tipo | Icono que indica si es transición de estado, documento adjunto, observación, etc. |
| Descripción | Texto descriptivo de la acción (ej: "Estado cambiado de EN REVISIÓN CAA → COTIZANDO") |
| Usuario | Nombre del usuario que realizó la acción |
| Observaciones | Texto libre ingresado por el usuario al ejecutar la acción (expandible) |

Filtros disponibles: por tipo de acción y por rango de fechas. Botón "Exportar historial" (Excel).

---

## 🖥️ Módulo 08: Gestión de Proveedores

### Propósito Funcional

Catálogo de proveedores habilitados para cotizar en solicitudes del HCG. Permite crear, editar y buscar proveedores. El Comprador DSA puede crear proveedores al registrar cotizaciones; el Administrador tiene acceso completo.

### Elementos de Interfaz

**Barra de Comandos:** Campo de búsqueda con autocompletado (por nombre o RFC), botón "+ Nuevo Proveedor".

**Lista de Proveedores:** Tabla paginada con columnas: Nombre o Razón Social, RFC, Contacto, Teléfono, Estado (Activo/Inactivo). Clic en fila abre el formulario de edición.

**Formulario de Proveedor:** Campos: Razón Social, RFC, Nombre de contacto, Teléfono, Correo electrónico, Dirección, Estado. Botones: Guardar, Cancelar.

---

## 🖥️ Módulo 09: Cotizaciones y Cuadro Comparativo

### Propósito Funcional

Registro de cotizaciones recibidas de proveedores para una solicitud activa y generación del cuadro comparativo que respalda la selección del proveedor (RN-003: mínimo 3 cotizaciones de proveedores distintos).

### Composición Visual

Dos sub-vistas accesibles desde pestañas: "Cotizaciones" y "Cuadro Comparativo".

**Sub-vista Cotizaciones:**

Tabla de cotizaciones registradas para la solicitud activa. Columnas: Proveedor, Monto Total, Días de Entrega, Condiciones, PDF (enlace al documento), Seleccionada. Botón "+ Agregar Cotización".

Indicador de progreso RN-003: `N de 3 cotizaciones requeridas` en color naranja mientras no se cumple el mínimo, verde una vez cumplido.

**Formulario de Nueva Cotización:** Campo de búsqueda de proveedor (busca en el catálogo de proveedores), monto, días de entrega, condiciones comerciales, adjunto PDF.

**Sub-vista Cuadro Comparativo:**

Tabla de comparación con las cotizaciones registradas: una fila por proveedor, columnas por criterio (monto, plazo, condiciones). Botón "Seleccionar Proveedor" junto a cada fila, deshabilitado si hay menos de 3 cotizaciones. Al seleccionar, modal de confirmación y la solicitud avanza al estado `PROVEEDOR_SELECCIONADO`.

---

## 🖥️ Módulo 10: Validación Fiscal — Panel CFDI

### Propósito Funcional

Panel exclusivo del rol **Finanzas** para gestionar la validación del Comprobante Fiscal Digital por Internet (CFDI 4.0) ante el SAT (RN-004), requisito previo al pago.

### Composición Visual

Panel de estado centrado con: estado actual del proceso de validación, área de carga del XML del CFDI, resultados de la validación y acciones disponibles.

### Estados del Panel CFDI

| Estado | Visual | Descripción |
|---|---|---|
| **Pendiente** | Ícono de documento + texto informativo | La solicitud está lista para iniciar la validación. Botón "Cargar CFDI". |
| **Cargando XML** | Indicador de carga | Se está procesando el archivo XML del CFDI. |
| **Validando ante SAT** | Indicador de carga + texto "Consultando al SAT..." | La API está validando el CFDI ante el servicio del SAT. |
| **CFDI Válido** | Ícono ✅ verde + datos del CFDI | El CFDI es auténtico. Botón "Confirmar y Avanzar al Pago". |
| **CFDI Inválido** | Ícono ❌ rojo + descripción del error | El CFDI no pasó la validación. Mensaje de error del SAT. Botón "Cargar CFDI Corregido". |
| **Error de Servicio SAT** | Ícono ⚠️ naranja | "El servicio del SAT no responde. Intente de nuevo más tarde." Botón "Reintentar". |

### Flujo Operativo

1. El usuario de Finanzas accede al panel desde el Detalle de la solicitud (estado `ENTREGADO`).
2. Carga el archivo XML del CFDI proporcionado por el proveedor.
3. Presiona "Validar ante SAT". El panel muestra el estado "Validando".
4. Si válido: el panel muestra los datos del CFDI y habilita "Confirmar y Avanzar al Pago". La solicitud pasa a estado `CFDI_VALIDADO`.
5. Si inválido: el panel muestra el error específico del SAT. El usuario puede cargar un CFDI corregido.
6. Si el SAT no responde: mensaje amigable y botón de reintento manual. El estado de la solicitud se mantiene en `ENTREGADO` hasta que la validación sea exitosa.

*Nota de implementación backend: La validación SAT se implementa con un `try/catch` estándar. Si la llamada al SAT falla por timeout o error de servicio, la API retorna un resultado de error con el mensaje correspondiente. No se requieren reintentos automáticos.*

---

## 🖥️ Módulo 11: Reportes y Exportación

### Propósito Funcional

Generación de reportes oficiales del proceso de Fondo Revolvente mediante el motor SSRS. Accesible a los roles con permisos de reporte según la Matriz de Visibilidad (Apéndice B).

### Elementos de Interfaz

**Lista de Reportes Disponibles:**

| Reporte | Roles con Acceso | Formatos |
|---|---|---|
| Solicitudes por período | Admin, Consulta DSA | PDF, Excel |
| Cuadro Comparativo de Cotizaciones | Admin, Comprador DSA | PDF |
| Expediente completo (por folio) | Admin, Comprador DSA (propias), CAA | PDF |
| Reporte financiero del período | Admin, Finanzas | PDF, Excel |
| Log de auditoría | Admin | Excel |

**Panel de Generación:** Para cada reporte, un formulario con los parámetros requeridos (período, folio, etc.) y un botón "Generar Reporte". Al presionar, el botón muestra un indicador de carga. Al completarse, aparecen los botones de descarga en PDF y/o Excel.

---

## 🖥️ Módulo 12: Panel de Administración — Roles y Accesos

### Propósito Funcional

Panel exclusivo del rol **Administrador**. Gestiona la asignación de roles, supervisa el historial de auditoría, configura los parámetros de negocio del sistema y monitorea el estado de los servicios integrados.

### Composición Visual

Sub-navegación con tres secciones: Usuarios y Roles, Auditoría de Acciones, Configuración del Sistema.

**Sub-sección: Usuarios y Roles:**

Campo de búsqueda de usuario (por nombre o usuario de red). Tabla de usuarios con columnas: Nombre Completo, Usuario de Red, Rol en la Aplicación (lista desplegable editable inline), Último Acceso, Estado (activo/inactivo con interruptor). Al modificar el rol, emerge un modal de confirmación.

**Sub-sección: Auditoría de Acciones:**

Filtros: rango de fechas, tipo de acción, usuario. Tabla de auditoría con columnas: Timestamp, Usuario, Tipo de Acción, Folio Afectado, Estado Anterior (StateBadge), Estado Nuevo (StateBadge), IP de Origen. Botón "Exportar Log" (Excel).

**Sub-sección: Configuración del Sistema:**

Parámetros editables con doble confirmación:

| Parámetro | Tipo | Restricciones |
|---|---|---|
| Monto Máximo Fondo Revolvente (MXN) | Campo numérico | Protegido. Requiere modal con campo de razón del cambio. |
| Mínimo de Cotizaciones (RN-003) | Campo numérico | Entero ≥ 1. Misma protección. |
| Tiempo de Expiración de Bloqueo (minutos) | Campo numérico | Rango 5-120 minutos. |
| Días en Estado para Activar Alerta | Campo numérico | Umbral para el Dashboard. |

**Panel de Estado de Servicios:**

| Servicio | Indicador |
|---|---|
| Servicio de Validación Fiscal (SAT) | Semáforo verde/rojo + timestamp última verificación exitosa |
| Servidor de Archivos SMB | Semáforo verde/rojo + ruta UNC + timestamp última escritura |

Si algún indicador está en rojo, se muestra un aviso de advertencia con botón "Intentar reconexión manual".

---

## 17. Especificación Transversal: Ciclo de Vida de la Solicitud (7 Estados)

### 17.1 Los 7 Estados del Ciclo de Vida

El sistema gestiona el ciclo de vida de una solicitud a través de **7 estados core**. Esta reducción desde los 30 estados originales elimina estados intermedios redundantes sin perder fidelidad al proceso real del HCG.

| Estado | Etiqueta UI | Color | Descripción |
|---|---|---|---|
| `BORRADOR` | Borrador | `#69797E` Gris | Solicitud creada pero no enviada a revisión. Solo visible para el Comprador DSA. |
| `EN_REVISION_CAA` | En Revisión CAA | `#FF8C00` Naranja | La solicitud fue enviada y está pendiente de autorización por el Comité CAA. |
| `COTIZANDO` | Cotizando | `#00B7C3` Cyan | CAA autorizó. El Comprador DSA está recabando cotizaciones de proveedores. |
| `PROVEEDOR_SELECCIONADO` | Proveedor Seleccionado | `#E74F2D` Naranja Cálido | Se completaron las cotizaciones y se seleccionó al proveedor. En espera de entrega. |
| `ENTREGADO` | Entregado | `#6DC253` Verde | Almacén confirmó la recepción de los bienes o servicios. |
| `CFDI_VALIDADO` | CFDI Validado | `#9B4F9E` Púrpura | Finanzas validó el CFDI ante el SAT. Listo para pago. |
| `PAGADO` | Pagado / Cerrado | `#107C10` Verde Oscuro | El pago fue ejecutado y el expediente está cerrado. |

**Estados terminales adicionales:**

| Estado | Etiqueta UI | Color | Descripción |
|---|---|---|---|
| `RECHAZADO_CAA` | Rechazado por CAA | `#C50F1F` Rojo | El CAA rechazó la solicitud. El Comprador puede ajustar y reenviar. |
| `CANCELADO` | Cancelado | `#C50F1F` Rojo | Solicitud cancelada por el Administrador. |

### 17.2 Transiciones Permitidas

```
BORRADOR ──────────────────────────────► EN_REVISION_CAA
EN_REVISION_CAA ────── (Autorizado) ──► COTIZANDO
EN_REVISION_CAA ────── (Rechazado) ───► RECHAZADO_CAA
RECHAZADO_CAA ──────── (Reenvío) ─────► EN_REVISION_CAA
COTIZANDO ─────────────────────────────► PROVEEDOR_SELECCIONADO
PROVEEDOR_SELECCIONADO ─────────────────► ENTREGADO
ENTREGADO ─────────────────────────────► CFDI_VALIDADO
CFDI_VALIDADO ─────────────────────────► PAGADO
[Cualquier estado activo] ─ (Admin) ──► CANCELADO
```

### 17.3 Comportamientos Visuales de Estados

| Condición | Tratamiento Visual |
|---|---|
| Estado activo (`EN_*`, `COTIZANDO`, etc.) | StateBadge con fondo en el color de la fase. |
| Estado que requiere acción del rol activo | Fila en la lista de solicitudes con fondo suave del color del estado. |
| Estado de error o rechazo | StateBadge con fondo en color Critical (rojo). |
| Estado terminal (`PAGADO`, `CANCELADO`) | Todos los elementos con opacidad reducida al 85%. |

---

## Apéndice A: Sistema de Colores por Estado

| Estado | Color Principal | Código Hex |
|---|---|---|
| `BORRADOR` | Gris Neutro | `#69797E` |
| `EN_REVISION_CAA` | Naranja | `#FF8C00` |
| `COTIZANDO` | Cyan | `#00B7C3` |
| `PROVEEDOR_SELECCIONADO` | Naranja Cálido | `#E74F2D` |
| `ENTREGADO` | Verde Claro | `#6DC253` |
| `CFDI_VALIDADO` | Púrpura | `#9B4F9E` |
| `PAGADO` | Verde Oscuro | `#107C10` |
| `RECHAZADO_CAA` | Rojo Crítico | `#C50F1F` |
| `CANCELADO` | Rojo Crítico | `#C50F1F` |

---

## Apéndice B: Matriz de Visibilidad de Módulos por Rol

| Módulo | Admin | Comprador DSA | CAA | Finanzas | Almacén | Consulta DSA |
|---|---|---|---|---|---|---|
| Autenticación | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Shell Principal | ✅ Completo | ✅ Filtrado | ✅ Filtrado | ✅ Filtrado | ✅ Filtrado | ✅ Solo lectura |
| Dashboard | ✅ Global | ✅ Sus solicitudes | ❌ | ❌ | ❌ | ✅ Global lectura |
| Lista de Solicitudes | ✅ Todas | ✅ Propias | ✅ Estado CAA | ✅ Estado CFDI/Pago | ✅ Estado entrega | ✅ Todas, lectura |
| Nueva Solicitud | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Detalle de Solicitud | ✅ + todas las acciones | ✅ + edición propias | ✅ + Autorizar/Rechazar | ✅ + CFDI/Pago | ✅ + Confirmar entrega | ✅ Solo lectura |
| Historial de Acciones | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ Solo lectura |
| Proveedores | ✅ + crear/editar | ✅ + crear | ❌ | ❌ | ❌ | ❌ |
| Cotizaciones | ✅ Completo | ✅ Completo | ❌ | ✅ Solo lectura | ❌ | ✅ Solo lectura |
| Validación CFDI | ✅ Completo | ❌ | ❌ | ✅ Completo | ❌ | ❌ |
| Reportes | ✅ Todos | ✅ Propios + comparativo | ✅ Expedientes CAA | ✅ Reportes financieros | ❌ | ✅ Todos, lectura |
| Administración | ✅ Completo | ❌ | ❌ | ❌ | ❌ | ❌ |

---

## Apéndice C: Representación Visual de las Reglas de Negocio

| Clave | Nombre | Representación Visual | Comportamiento al Incumplimiento |
|---|---|---|---|
| **RN-001** | Monto máximo $75,000 MXN | MontoDisplay con barra de progreso coloreada (verde → amarillo → rojo). | Aviso de advertencia emerge. Botón "Registrar Solicitud" se deshabilita. |
| **RN-002** | Prohibición de fraccionamiento | Aviso de advertencia cuando el sistema detecta el patrón en los códigos de producto. | Campo de justificación obligatorio y casilla de confirmación explícita. El botón de registro permanece deshabilitado hasta confirmar. |
| **RN-003** | Mínimo 3 cotizaciones | Indicador de progreso `N de 3 cotizaciones` (naranja mientras no se cumple, verde al cumplir). | El botón "Confirmar Selección de Proveedor" permanece deshabilitado hasta registrar ≥ 3 cotizaciones. |
| **RN-004** | CFDI válido para proceder al pago | Panel CFDI con estados visuales claros: Pendiente, Validando, Válido, Inválido, Error de servicio SAT. | Si inválido: aviso de error con descripción del SAT. Si el servicio SAT no responde: aviso amigable con botón "Reintentar". |
| **RN-005** | Bloqueo de edición (un editor a la vez) | BloqueoIndicator en el encabezado del expediente cuando hay un bloqueo activo. Modal informativo al intentar editar un expediente bloqueado. | El expediente se abre en modo solo lectura. Solo las acciones de edición se bloquean. |

---

## Apéndice D: Glosario de Términos

| Término | Definición |
|---|---|
| **Solicitud** | Expediente de adquisición creado por un Comprador DSA que pasa por los 7 estados del proceso de Fondo Revolvente. Unidad fundamental del sistema. |
| **Folio DSA** | Identificador único alfanumérico de cada solicitud. Formato: `DSA-AAAA-NNN` (ej: DSA-2026-089). Generado automáticamente al registrar la solicitud. |
| **CFDI** | Comprobante Fiscal Digital por Internet (versión 4.0). Documento fiscal electrónico emitido por el proveedor. Requerido para autorizar el pago. |
| **CAA** | Comité de Adquisiciones y Arrendamientos. Órgano colegiado del HCG que autoriza solicitudes de Fondo Revolvente. |
| **RN-001** | Regla de Negocio 1: El monto total de una solicitud no puede exceder $75,000 MXN. |
| **RN-002** | Regla de Negocio 2: Prohibición de fraccionamiento de adquisiciones. |
| **RN-003** | Regla de Negocio 3: Se requieren mínimo 3 cotizaciones de proveedores distintos. |
| **RN-004** | Regla de Negocio 4: El CFDI del proveedor debe ser validado ante el SAT antes del pago. |
| **RN-005** | Regla de Negocio 5: Solo un usuario puede editar un expediente a la vez. El bloqueo es un flag en la base de datos con expiración de 30 minutos. |
| **StateBadge** | Componente visual reutilizable que representa el estado de una solicitud con color, ícono y texto. Disponible en variantes Small, Medium y Large. |
| **BloqueoIndicator** | Componente visual que indica que un expediente tiene un bloqueo de edición activo, mostrando quién lo tiene y desde qué hora. |
| **MontoDisplay** | Componente de visualización monetaria que formatea valores como `$XX,XXX.XX MXN` e incluye una barra de progreso respecto al límite de $75,000 MXN. |
| **MVVM** | Model-View-ViewModel: patrón de arquitectura de UI que separa la lógica de presentación (ViewModel) de la vista, habilitando reactividad automática. |
| **Polling** | Técnica donde el cliente consulta la API periódicamente (cada 30 segundos) para detectar cambios en los datos, en sustitución de comunicación en tiempo real por WebSockets. |
| **SMB** | Server Message Block: protocolo de red para compartir archivos en red local. El hospital usa SMB 3.0 para el repositorio de expedientes en `\\Servidor\Compras_FR\`. |
| **SSRS** | SQL Server Reporting Services: motor de reporteo que genera documentos PDF y Excel a partir de plantillas y datos de SQL Server. |
| **SAT** | Servicio de Administración Tributaria: autoridad fiscal de México. La aplicación consulta al SAT para validar la autenticidad de los CFDIs emitidos por proveedores. |

---

**Fin del Documento de Especificación Funcional y de Interfaz — Edición Práctica v1.0**
