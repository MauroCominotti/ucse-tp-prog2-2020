# ucse-tp-prog2-2020
<<<<<<< HEAD
## Trabajo práctico de Programación 2

Se necesita desarrollar un sistema para administrar instituciones educativas.

**Aspectos funcionales:**

Cada institución educativa cuenta con diferentes personas que interactúan entre sí durante el ciclo lectivo, por lo que la idea es desarrollar un sistema que facilite dicha comunicación.

En primer lugar una institución cuenta con un director, del cual conocemos sus datos personales.
Luego existe el perfil de docentes, donde cada uno de ellos está asociado a una o más salas de un único establecimiento educativo.

Luego existe el perfil de padres, de los cuales aparte de los datos personales, necesitamos conocer la lista de hijos que tiene en cada establecimiento educativo.

Y por último tenemos el perfil de alumnos (que son los hijos) que no pueden entrar al sistema por sí solos, por lo que deberemos desarrollar la aplicación para que sean los padres quienes administren la información de cada uno de sus hijos.

En este primer módulo se desarrollará toda la lógica necesaria para que docentes, directores y padres interactúen mediante notas de un cuaderno de comunicaciones digital.

**Contexto técnico.**

**El contexto de este trabajo práctico implica el desarrollo de toda la lógica de negocios y acceso a datos, así como también una capa de servicios WCF que permita ser consumida por una aplicación web que entregaremos en su momento.**

Se requiere:

  1) ABM de Directores
  2) ABM de Docentes (asignación de salas)
  3) ABM de Alumnos
  4) Que un docente / director pueda dar de alta una nota a un alumno particular
  5) Que un padre / docente / director pueda responder una nota según si:
    a) Si es padre y la nota corresponde a uno de sus hijos
    b) Si es docente y la nota corresponde a un alumno de su sala
    c) Si es director y la nota corresponde a un alumno de su institución
  6) Que un padre pueda marcar una nota como leída.

Se entregará un repositorio base con la interface que debe implementar el servicio y la parte web resuelta.

Cada pantalla de grilla contará con filtros para buscar notas.

Se deberá controlar de alguna manera el acceso a la aplicación, mediante un login, que permita luego saber qué usuario está conectado y su rol en el sistema.

**Aspectos técnicos y consideraciones.**

Se valorará el modelado de la solución así como también aspectos como legibilidad de código, mantenibilidad, denominación de variables y métodos, y buenas prácticas de desarrollo.
Se deben implementar al menos 3 eventos con sus respectivos delegados.
Se deben implementar test unitarios en al menos 2 clases.


**Repositorio base:**
TBD

Se utilizará la estrategia de fork para poder mantener actualizadas las ramas de web y lógica y utilizaremos Github como herramienta de gestión de versiones.

Registrarse en github y seguir el siguiente tutorial:
https://frontendlabs.io/3266--que-es-hacer-fork-repositorio-y-como-hacer-un-fork-github
=======
TP prog2 año 2020

```csharp
/// <summary>
/// Nombre de los integrantes del grupo de trabajo
/// </summary>
/// <returns></returns>
string ObtenerNombreGrupo();

/// <summary>
/// Retorna un usuario logueado a partir de sus credenciales
/// </summary>
/// <param name="email"></param>
/// <param name="clave"></param>
/// <returns></returns>
UsuarioLogueado ObtenerUsuario(string email, string clave);

/// <summary>
/// Obtiene un listado de instituciones guardada
/// </summary>
/// <returns></returns>
Institucion[] ObtenerInstituciones();
/// <summary>
/// El usuario logueado debe ser una directora del mismo institucion
/// </summary>
/// <param name="directora"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AltaDirectora(Directora directora, UsuarioLogueado usuarioLogueado);

/// <summary>
/// El usuario logueado debe ser una directora
/// </summary>
/// <param name="hijo"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AltaAlumno(Hijo hijo, UsuarioLogueado usuarioLogueado);

/// <summary>
/// El usuario logueado debe ser una directora
/// </summary>
/// <param name="id"></param>
/// <param name="hijo"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EditarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado);

/// <summary>
/// El usuario logueado debe ser una directora
/// </summary>
/// <param name="id"></param>
/// <param name="hijo"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EliminarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado);

/// <summary>
/// El usuario logueado debe ser una directora del mismo institucion
/// </summary>
/// <param name="directora"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EditarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario logueado debe ser una directora del mismo institucion
/// </summary>
/// <param name="directora"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EliminarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado);
/// <summary>
/// Las salas son de la institucion del usuario logueado
/// </summary>
/// <param name="institucion"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Sala[] ObtenerSalasPorInstitucion(UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario logueado debe ser una directora del mismo institucion
/// </summary>
/// <param name="docente"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AltaDocente(Docente docente, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario logueado debe ser una directora del mismo institucion
/// </summary>
/// <param name="id"></param>
/// <param name="docente"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EditarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario logueado debe ser una directora del mismo institucion
/// </summary>
/// <param name="id"></param>
/// <param name="docente"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EliminarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario debe ser directora del mismo institucion
/// </summary>
/// <param name="padre"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AltaPadreMadre(Padre padre, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario debe ser directora del mismo institucion
/// </summary>
/// <param name="padre"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EditarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario debe ser directora del mismo institucion
/// </summary>
/// <param name="padre"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado EliminarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario debe ser directora, y tanto la sala como el docente deben pertenecer a su institucion.
/// </summary>
/// <param name="docente"></param>
/// <param name="sala"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AsignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado);
/// <summary>
/// 
/// </summary>
/// <param name="docente"></param>
/// <param name="sala"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado DesasignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado);
/// <summary>
/// El usuario debe ser directora, y el hijo debe estar asociado a una sala de su institucion
/// </summary>
/// <param name="hijo"></param>
/// <param name="padre"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AsignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado);
/// <summary>
/// 
/// </summary>
/// <param name="hijo"></param>
/// <param name="padre"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado DesasignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado);
/// <summary>
/// Si el usuario es directora, retornar alumnos de la institucion, si es docente los de sus salas, y si es padre solo sus hijos.
/// </summary>        
/// <returns></returns>
Hijo[] ObtenerPersonas(UsuarioLogueado usuarioLogueado);
/// <summary>
/// Obtiene las notas de un cuaderno, si el usuario es padre solo puede obtener cuadernos de sus hijos, si es docente de alumnos de sus salas
/// y si es directora de cualquier alumno de la institucion
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Nota[] ObtenerCuadernoComunicaciones(int idPersona, UsuarioLogueado usuarioLogueado);
/// <summary>
/// Alta de una nota, la nota puede estar dirigida a 1 o varias salas, o 1 o varios alumnos. Si el usuario es padre solamente podra enviar a sus hijos.
/// </summary>
/// <param name="nota"></param>
/// <param name="salas"></param>
/// <param name="hijos"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado AltaNota(Nota nota, Sala[] salas, Hijo[] hijos, UsuarioLogueado usuarioLogueado);
/// <summary>
/// Respuesta a una nota. Si es docente la nota debe ser de un alumno de la sala
/// </summary>
/// <param name="nota"></param>
/// <param name="nuevoComentario"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado ResponderNota(Nota nota, Comentario nuevoComentario, UsuarioLogueado usuarioLogueado);
/// <summary>
/// Marca la nota como leida si le corresponde.
/// </summary>
/// <param name="nota"></param>
/// <param name="usuarioLogueado"></param>
/// <returns></returns>
Resultado MarcarNotaComoLeida(Nota nota, UsuarioLogueado usuarioLogueado);

/// <summary>
/// Grilla de directoras
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="paginaActual"></param>
/// <param name="totalPorPagina"></param>
/// <param name="busquedaGlobal"></param>
/// <returns></returns>
Grilla<Directora> ObtenerDirectoras(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal);

/// <summary>
/// Grilla de docentes
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="paginaActual"></param>
/// <param name="totalPorPagina"></param>
/// <param name="busquedaGlobal"></param>
/// <returns></returns>
Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal);

/// <summary>
/// Grilla de padres
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="paginaActual"></param>
/// <param name="totalPorPagina"></param>
/// <param name="busquedaGlobal"></param>
/// <returns></returns>
Grilla<Padre> ObtenerPadres(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal);

/// <summary>
/// Grilla de alumnos
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="paginaActual"></param>
/// <param name="totalPorPagina"></param>
/// <param name="busquedaGlobal"></param>
/// <returns></returns>
Grilla<Hijo> ObtenerAlumnos(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal);

/// <summary>
/// Obtener directora por ID
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="id"></param>
/// <returns></returns>
Directora ObtenerDirectoraPorId(UsuarioLogueado usuarioLogueado, int id);

/// <summary>
/// Obtener docente por ID
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="id"></param>
/// <returns></returns>
Docente ObtenerDocentePorId(UsuarioLogueado usuarioLogueado, int id);

/// <summary>
/// Obtener padre por ID
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="id"></param>
/// <returns></returns>
Padre ObtenerPadrePorId(UsuarioLogueado usuarioLogueado, int id);

/// <summary>
/// Obtener hijo por ID
/// </summary>
/// <param name="usuarioLogueado"></param>
/// <param name="id"></param>
/// <returns></returns>
Hijo ObtenerAlumnoPorId(UsuarioLogueado usuarioLogueado, int id);
>>>>>>> fa5c35a187d8ddb6c89a1bd3b01f508233edba7f
