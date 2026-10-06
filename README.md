# Hacker News API 

La solución entregada contiene exactamente HackerNews.Api y HackerNews.Tests. Requiere SDK .NET 8. No requiere Python ni base de datos ni generación manual de credenciales para ejecutar la demostración.

La solución consta de dos proyectos: HackerNews.Api (donde se encuentra la API desarrollada según lo solicitado en el Developer Coding Test) y HackerNews.Tests (donde se encuentran tests unitarias y de integración).

La API fue desarrollada incluyendo los siguientes conceptos: 

- La API respeta los principios RESTful API. 

- Se aplicaron buenas prácticas de desarrollo como SOLID.

- Las URLs de consumo de datos externos del proyecto son configurables desde el web.config y el appsettings, dependiendo del ambiente.

- Se utilizaron las siguientes librerías: Autofac, AutoMapper, FluentValidation, Swagger, Serilog. 

- La API se desarrolló siguiendo una arquitectura del tipo repositorio, incorporando el patrón de diseño Mediator (MediatR). 

- La API incluye políticas de reintento, cacheo de respuesta, envío en tandas de consultas a la API externa (para evitar sobrecargar la API de Hacker News) y manejo de errores globales.

- Fue implementado JWT en el proyecto, donde para poder ejecutar una llamada al controlador HackerNewsController se deberá solicitar un token en el controlador. AuthorizationController (donde se deberá

Utilizar las siguientes credenciales para obtener un token para validarse contra el HackerNewsController: User: demo, password: demo.

- Se implementó Swagger en el proyecto, por lo cual se podrá probar su funcionalidad mediante este medio.

- Se generó su Dockerfile para poder generar su contenerización. 

- Se incluye el uso de middleware para: Logging avanzado con Serilog y manejo global de excepciones.

En la API se incluyeron dos controladores:

AuthorizationController

	Donde existe un método del tipo GET (/api/authorization/token) que requiere dos parámetros: Usuario y password (esto se realizó para imitar lo que sería el pedido del front end de un token válido).

	Para realizar la solicitud de token válido para realizar la autorización del segundo controlador HackerNewsController, si las credenciales son válidas, devolverá un token válido con el siguiente formato:

	 {

	  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZW1vIiwianRpIjoiNzY4OWI3YjEtMTU0My00MjIzLWIxYTUtZjI3N2VmNDQ2ZWViIiwibmJmIjoxNzkxMzA5Nzg2LCJleHAiOjE3OTEzMTE1ODYsImlzcyI6IkhhY2tlck5ld3MiLCJhdWQiOiJIYWNrZXJOZXdzQ2xpZW50cyJ9.YjMUDQMdoqnQpxb8dMOz-TLhV3DvS9jzKcOyWuPUO1U",

	  "tokenType": "Bearer"

	  "expiresIn": 1800

	 }

	Para realizar la autorización, se deberá copiar por completo el parámetro: accessToken (eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZW1vIiwianRpIjoiNzY4OWI3YjEtMTU0My00MjIzLWIxYTUtZjI3N2VmNDQ2ZWViIiwibmJmIjoxNzkxMzA5Nzg2LCJleHAiOjE3OTEzMTE1ODYsImlzcyI6IkhhY2tlck5ld3MiLCJhdWQiOiJIYWNrZXJOZXdzQ2xpZW50cyJ9.YjMUDQMdoqnQpxb8dMOz-TLhV3DvS9jzKcOyWuPUO1U)).

	Métodos controlador: GetToken

		

HackerNewsController

	En este contador existe un método del tipo GET (/api/hackernews/best-stories) que requiere de un parámetro N (que identifica la cantidad de historias a ser devueltas en la respuesta).

	En caso de que el usuario se haya autenticado correctamente, se obtendrá una respuesta de este tipo, pasándole un N = 2:

	 Ejemplo:

	

	 [

	  {

		"title": "Mistral Large 4"

		"uri": "https://mistral.ai/news/mistral-large-4/\\",

		"postedBy": "Philpax"

		"time": "2026-10-06T13:15:49+00:00",

		"score": 1037,

		"commentCount": 111

	  },

	  {

		"title": "Anthropic reported diary entry to police; woman faces felony charge"

		"uri": "https://www.techspot.com/news/114091-florida-woman-used-claude-diary-anthropic-reported-shoot.html",

		"postedBy": "emptybits",

		"time": "2026-10-05T05:37:40+00:00",

		"score": 782,

		"commentCount": 99

	     }

	 ]

