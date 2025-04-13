Yksinkertainen lentolippujärjestelmä, jossa normaalina käyttäjänä voi hakea lentoja ja varata itselleen lentolipun. Admin-käyttäjä voi lisätä, muokata tai poistaa lentoja.

Automatisoidut testit eivät ole valmiita (eivätkä ole vielä repossa).

Hakemistorakenne:
Models-kansio sisältää ohjelman datarakenteen, kuten käyttäjät, lennot ja varaukset.<br />
Services-kansio sisältää tietokannan- ja salasananhallinnan.<br />
Views-kansio sisältää käyttöliitymäsivut ja koodit niiden taustalla.<br />

Vaatii toimiakseen PostgreSQL:n ja .NET Maui:n. Tietokannan tiedot löytyvät Services/DatabaseService.cs -koodista (_connectionString). db.sql -tiedoton avulla voi luoda tarvittavat taulut.

Ohjelman ajaminen: Siirry ensiksi hakemistoon, jossa Lentolippujärjestelmä.csproj sijaitsee (./Lentolippujärjestelmä). Tässä hakemistossa anna komennot "dotnet build" ja sitten "dotnet run --framewrok net9.0-windows10.0.19041.0

Ohjelman tekemisessä on hyödynnetty tekoälyä.
