# Microservice/Multi service project. I made this process only to play around with domain driven development and GRPC and bunch of other stuff
- ApplicationApi is the consumer facing API
It has Application as Model and aggregate root, it represents a job application and has references to one person and multiple documents. API has normal CRUD endpoints and 2 domain specific endpoints:
  NextStep - moves the applicant to next step in hiring process
  Reject - kicks the applicant out of hiring process
- DocumentsService is a GRPC api of Documents for a person and it has one model, Document that represents applicant's document ( cv, motivation lettter, certificate, etc ). It has one endpoint get that returns a list of documents for a person (input is person Id) 
- PersonsService is a GRPC api of Persons and it has one model, Person that represents a persons ( name, lastname, email, etc ). It has one endpoint getById that returns a person by Id

DocumentsService and PersonsService don't have persistent data, its just List<T>, this is a toy project so i didn't bother ( ApplicationAPI does have persistent db tho ).
