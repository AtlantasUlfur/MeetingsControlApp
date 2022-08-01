# ConsoleUI
This is a console application to control meetings of your company
## Technology stack
- .NET 6
- C# 10
### To start 

set "People_file" and "Meetings_file" in appsettings.json file to existing .json files on your local machine.

compile and run the program

Test User:  
username: root  
password: root  

### Available commands

```http request
-h
```
Shows all available commands

```http request
GET * people
```
Returns all people from the "database"

```http request
GET * meetings
```
Returns all meetings from the "database"

```http request
[Create/Add] new meeting
```
Prompts you to input information to add a new meeting to the "database"

```http request
Delete meeting {meeting_id}
```
Input parameters:
`meeting_id` - integer

Deletes a meeting from the "database"


```http request
ADD person {person_id} TO meeting {meeting_id}
```
Input parameters:
`person_id` - integer
`meeting_id` - integer

Adds a person to a meeting

```http request
[DELETE/REMOVE] person {person_id} FROM meeting {meeting_id}
```
Input parameters:
`person_id` - integer
`meeting_id` - integer

Removes person from a meeting

```http request
FILTER meetings
```

Prompts to add a filter to show meetings

## Still needed:

- Unit testing
- A lot of refactoring
- Adding comments for methods
- Architecture overhaul
- Error handling improvements
- Code culture improvement
- etc..