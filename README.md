# Tinyurl 
REST API web service written in C# with .Net 6 framework, allows users to short URLs, monitor, track after them, and
see statistics.

## Technologies
* **Backend**: .Net core 6 - C#
* **Database**:
    * **MongoDB** - for storing users and urls data related to them (total amount of clicks, total amount of clicks per month and year, etc)
    * **Redis** - for storing and mapping short url to long url (key-value)
    * **PostgreSql** - for storing and logging user's activity about url visits (when, which url,etc.)
* **Containerization**: Docker
* **Authentciation**: JWT 

# Features
* **Short url** - user can create short url for any url
* **Url statistics** - user can see statistics about his urls via mongodb
* **Url activity** - user can see activity and entrances about his urls.
* **User registration** - user can register with email and password.
* **User login** - user can log in with email and password

