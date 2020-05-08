# ucse-tp-prog2-2020
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
