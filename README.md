# NetVsStar
### Proyecto en conjunto utilizado para Títulos de Ingeniería en Ejecución y Computación Informática y de Ingeniería Civil Informática UBB

## Despliegue del proyecto

Se debe utilizar Unity 2020.3.32.f1 para abrir el proyecto. Para eso debe descargar Unity Hub, importar el proyecto y abrirlo con el programa, cuando lo haga, Unity le 
indicará que debe instalar la versión necesaria para abrir el proyecto y le dará la posibilidad de hacerlo, una vez instalada puede abrir el proyecto.

Una vez dentro, en la pestaña de Proyecto debe ir a la carpeta "Scenes" y hacer doble click en "RewardSearchTraining" para entrar a la escena principal.

## Pruebas: Partidas con jugador humano

En la pestaña "Hierarchy" verá varios objetos, los más relevantes para realizar pruebas son "HumanPlayerGame" y "NetPlayerGame" en donde uno estará en letras gris oscuro
mientras que el otro no. Si desea realizar una prueba con un jugador humano, debe hacer click en "HumanPlayerGame", revisar la pestaña de "Inspector" y verificar que tenga
una checkbox con un tick al lado del nombre del objeto, eso significa que este estará activo, debe verificar que la misma checkbox en el objeto "NetPlayerGame" esté desmarcado
dado que para realizar pruebas correctamente se debe tener activo uno de estos dos objetos a la vez y no ambos. Debe tomar en cuenta que al reproducir este modo de juego
se crearán automáticamente dos archivos de texto en la carpeta principal "Assets" con los datos que hay en cada movimiento que realice.

Luego, dentro del objeto "HumanPlayerGame" verá un objeto llamado "GameManager", si le hace click y revisa la pestaña de "Inspector" verá un componente llamado "TraceModules"
que tiene una propiedad llamada "File Number", debe asignarle un valor numérico entero, verificando que no existen datos marcados con ese número, caso contrario añadirán 
turnos a archivos ya existentes, si sólo desea experimentar libremente puede asignarle un número alejado de la cantidad de entrenamiento, por ejemplo 500, si desea añadir
una partida completa nueva para entrenar a la red, debe verificar que el número será el siguiente a los ya existentes, por ejemplo, si existen partidas hasta el número 40,
debe asignar el número 41.

Finalmente, presione el botón "Play" en la parte superior de la pantalla para ejecutar el modo de juego, en donde podrá mover al "pacman" (representado por un cuadrado azul)
con las flechas del teclado.

## Pruebas: Partidas con red neuronal

Para el caso de querer probar la red neuronal debe verificar que "HumanPlayerGame" está inactivo (Su checkbox está desmarcado) y "NetPlayerGame" está activo, debe hacer click
al objeto "GameManager" que está dentro de "NetPlayerGame" y verificar la propiedad "TrainingAmount" del componente "Manager", este debe tener un número entero menor o igual
a la cantidad de partidas que tiene registradas, finalmente presione el botón "Play" para entrar al modo de juego, donde el "pacman" estará siendo manejado por la red neuronal.

## Consideraciones

Las partidas quedan registradas en dos archivos diferentes, estos tiene por nombre "ClosestRewardEveryTurnX" y "TraceDataX" donde X en ambos casos representan un número entero
que representan la partida que se ha jugado, estos archivos contienen datos de cada turno en formato JSON.
Además, las recompensas en el mapa no cambian de posición de forma automática, debe moverlas manualmente en el editor si desea probar con otras disposiciones de recompensas.
Finalmente, el mapa contiene 4 recompensas, si desea quitar o agregar más, deberá reestructurar la red neuronal y los datos de entrenamiento, por lo que si agrega alguna
recompensa adicional será ignorada totalmente por la red.
