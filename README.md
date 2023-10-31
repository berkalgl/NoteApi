# NoteApi

- Backend Service as a REST API for a note taking application.
- Within this application the following tasks are viable: Create,
read, edit and delete notes.

## Used Technologies

● .Net 7

● EntityFrameworkCore

● InMemoryDb

● FluentValidation

● Swagger

## How to run
1- git clone https://github.com/berkalgl/NoteApi.git

2- cd .\NoteApi\

3- docker-compose up

4- NoteApi http://localhost:5001/swagger/index.html

5- AuthApi http://localhost:5002/swagger/index.html


- Doc
- 
There are two microservices: NoteApi and AuthApi

Applications are using basic auth with Jwt bearer token to handle authorization and authentication

AuthApi 

- is used to create jwt token to implement authorization in our application, and it is also responsible for editing and reading users.
- The api has already registered three users while application is running.
 1) Email = "berkAdmin@mail.com", Password = "berkAdmin", Role = Administrator
 2) Email = "berkEditor@mail.com", Password = "berkEditor", Role = Editor
 3) Email = "berkReader@mail.com", Password = "berkReader", Role = Reader

 There are four endpoints in the AuthApi, 
- api/v1/login is to log the user in our system. It only requires the email and password of the user.
Request:
 {
  "email": "string",
  "password": "string"
}

	Possible Responses: 200, 400, 401, 500


- Users controller is responsible for updating and reading existing users. Only a user with the role of ‘Admin’ can do the changes.
- First, the user has to login our system and receive the token then the user can call the existing endpoints. 
a decoded token:
{
  "sub": "1", // user id
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Administrator", // user role
  "exp": 1698766999,
  "iss": "dummy.com.tr",
  "aud": "dummy.com.tr"
}


- NoteApi 

- has only 5 endpoints to handle user notes requests.
- Post and put method can be only called by a user which has Administrator or Editor role
- Delete method can be only called by a user which has Administrator role
- Get All and Getting only one note method can be called by all users which has Administrator or Editor or Reader role