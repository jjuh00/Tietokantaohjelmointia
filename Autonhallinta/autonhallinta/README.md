JavaFX-ohjelma, jossa voi tarkastella, muokata, lisätä, hakea ja poistaa autoja. Vaatii toimiakseen MongoDB:n, Javan ja Mavenin. Testattu toimivaksi Java ja JavaFX versiolla 21.

Hakemistorakenne: <br />
Kansiossa Autonhallinta/autonhallinta sijaitsee kuvakaappaukset, tietokannan sisältö (db.json) ja pom.xml (HUOM! rivillä 60 muokkaa polku siihen kansioon, jossa java.exe sijaitsee). <br />
Kansiossa Autonhallinta/autonhallinta/src/main/java on module-info.java -koodi.<br /> ...java/com/autonhallinta -kansiossa sijaitsee ohjelman Java-koodi. DatabaseService.java sisältää MongoDB:hen liittyvän koodin, kuten tietokannan ja kokoelman nimet. <br />
Kansiossa Autonhallinta/autonhallinta/src/main/resources/com/autonhallinta on käyttöliittymäkoodia.

Ohjelma ajetaan komennolla mvn javafx:run tai mvn clean javafx:run. Anna komento hakemistossa Autonhallinta/autonhallinta.

Tietokannanhallinnassa ja käyttöliitymän tekemisessä on hyödynnetty tekoälyä.
