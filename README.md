# Currency exchange app
Application is built using Angular 10 for front-end and .net core 3.1 for back-end api. To run application clone the repository and follow steps below.

## UI 
Run npm-install in CurrencyExchange-UI directory. Once it's finished you should be able to run ng serve and access application at http://localhost:4200. 
Angular Material is used for frontend components. environment.ts contains the url of the local api - make sure the port matches your running api's port.

## API
Build and run CurrencyExchange.API Web API project. Application fetches exchange rates from ECB on startup and inserts them into an in-memory database.
After the initial insert, exchange rates are refreshed according to a [CRON](https://crontab.guru) schedule provided in appsettings.json. Default is everyday at 17:30. There you can also configure endpoint URL from which data is taken. Refresh of data is done using a background worker service. 
Xunit/Moq combination used for testing.
