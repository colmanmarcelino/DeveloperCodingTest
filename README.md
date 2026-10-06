# Hacker News API 

La solución entregada contiene exactamente HackerNews.Api y HackerNews.Tests. Requiere SDK .NET 8. No requiere Python, .env, base de datos ni generación manual de credenciales para ejecutar la demostración.
La solcuion consta de dos proyectos HackerNews.Api (donde se encuentra la api desarrollada segun lo solicitado en el Developer Coding Test) y HackerNews.Tests (donde se encuentran test unitarias y de integracion )

La api fue desarrollada incluyendo lo siguiente conceptos: 
- La api respeta los principios RESTful API 
- Se aplicaron buenas practicas de desarrollo como solid
- Las url de consuumo de datos externos del proyecto son configurables desde el web config y el appsetting dependendiendo del ambiente.
- Se utilizaron las siguientes librerias: autofac, automapper, FluentValidation, Swagger, Serilog 
- La api se desarrollo siguiendo una arquitectura del tipo repositorio incorporando el patron de diseño mediator (MediatR). 
- La api incluye politicas de reintento, cacheo de respuesta, envio en tandas de consultas a la api externa (para evitar sobrecargar la API de Hacker News) y manejo de errores globales
- Fue implementado JWT en el proyecto donde para poder ejecutar una llamada al contralador HackerNewsController se debera solicitar un token en el contralador: AuthorizationController (donde se debera
utilziar las siguiente credenciales para obtener un toker para validarse contra el HackerNewsController ) User: demo password: demo
- Se implemento Swagger en el proyecto por lo cual se podra probar sus funcionalidad mediante este medio.
- Se genero su docker file para poder generar su contenerización 
- Se incluye el uso de middleware para: Logging avanzado con Serilog y Manejo global de excepciones.


En la api se inlcuyo dos contraladores:

AuthorizationController
	Donde existe un metodo del tipo get (/api/authorization/token) que requiere dos parametros: Usuario y password (esto se realizo para imitar lo que seria el pedido del front end de un token valido)
	para realizar la solicitud de token valido para realizar la autorizacion del segundo contralador HackerNewsController, si las credenciales son validas devolvera un token valido con el siguiente formato:
	{
	  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZW1vIiwianRpIjoiNzY4OWI3YjEtMTU0My00MjIzLWIxYTUtZjI3N2VmNDQ2ZWViIiwibmJmIjoxNzkxMzA5Nzg2LCJleHAiOjE3OTEzMTE1ODYsImlzcyI6IkhhY2tlck5ld3MiLCJhdWQiOiJIYWNrZXJOZXdzQ2xpZW50cyJ9.YjMUDQMdoqnQpxb8dMOz-TLhV3DvS9jzKcOyWuPUO1U",
	  "tokenType": "Bearer",
	  "expiresIn": 1800
	}
	(para realizar la autorizacion se debera copiar por completo el parametro: accessToken (eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZW1vIiwianRpIjoiNzY4OWI3YjEtMTU0My00MjIzLWIxYTUtZjI3N2VmNDQ2ZWViIiwibmJmIjoxNzkxMzA5Nzg2LCJleHAiOjE3OTEzMTE1ODYsImlzcyI6IkhhY2tlck5ld3MiLCJhdWQiOiJIYWNrZXJOZXdzQ2xpZW50cyJ9.YjMUDQMdoqnQpxb8dMOz-TLhV3DvS9jzKcOyWuPUO1U))
	Meotodos controlador: GetToken
		
HackerNewsController
	En este contrador existe un metodo del tipo get (/api/hackernews/best-stories) que requiere de un parametro N (que identifica la cantidad de historias a ser devueltas en la respuesta)
	En caso de que el usuario se haya autenticado correctamente se obtendra una respuesta de este tipo pasandole un N = 2:
	Ejemplo:
	
	[
	  {
		"title": "Mistral Large 4",
		"uri": "https://mistral.ai/news/mistral-large-4/\\",
		"postedBy": "Philpax",
		"time": "2026-10-06T13:15:49+00:00",
		"score": 1037,
		"commentCount": 111
	  },
	  {
		"title": "Anthropic reported diary entry to police, woman faces felony charge",
		"uri": "https://www.techspot.com/news/114091-florida-woman-used-claude-diary-anthropic-reported-shoot.html",
		"postedBy": "emptybits",
		"time": "2026-10-05T05:37:40+00:00",
		"score": 782,
		"commentCount": 99
	  }
	]
