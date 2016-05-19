# Tavis.HttpResponseMachine

This library provides a declarative way of handling HTTP responses.


Responses can be dispatched based on status code, media type and link relation type.

		machine.When(HttpStatusCode.OK, null, new MediaTypeHeaderValue("application/json"))
                .Then(async (l, r) =>
            {
                var text = await r.Content.ReadAsStringAsync();
                root = JToken.Parse(text);
            });

Declarative parsers can be defined to map HttpContent to strongly typed objects

       parserStore.AddMediaTypeParser<JToken>("application/json", async (content) =>
            {
                var stream = await content.ReadAsStreamAsync();
                return JToken.Load(new JsonTextReader(new StreamReader(stream)));
            });

            // Define method to translate media type DOM into application domain object instance based on profile
            parserStore.AddProfileParser<JToken, Person>(new Uri("http://example.org/person"), (jt) =>
            {
                var person = new Person();
                var jobject = (JObject)jt;
                person.FirstName = (string)jobject["FirstName"];
                person.LastName = (string)jobject["LastName"];

                return person;
            });

            var machine = new HttpResponseMachine(parserStore);

            // Define action in HttpResponseMachine for all responses that return 200 OK and can be translated somehow to a Person
            machine
                .When(HttpStatusCode.OK)
                .Then<Person>((m, l, p) => { testPerson = p; });


