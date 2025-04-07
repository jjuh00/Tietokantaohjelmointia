package com.autonhallinta;

import com.mongodb.client.*;
import com.mongodb.client.model.Filters;
import com.mongodb.client.model.Updates;
import org.bson.Document;
import org.bson.types.ObjectId;
import org.bson.conversions.Bson;

import java.util.List;
import java.util.ArrayList;
import java.util.regex.Pattern;

public class DatabaseService {
    private static final String CONNECTION_STRING = "mongodb://localhost:27017";
    private static final String DATABASE_NAME = "car_management";
    private static final String COLLECTION_NAME = "cars";
    
    private MongoClient mongoClient;
    private MongoDatabase database;
    private MongoCollection<Document> carsCollection;

    private static DatabaseService instance;
    
    public static DatabaseService getInstance() {
        if (instance == null) {
            instance = new DatabaseService();
        }
        return instance;
    }

    private DatabaseService() {
        connect();
    }

    private void connect() {
        try {
            mongoClient = MongoClients.create(CONNECTION_STRING);
            database = mongoClient.getDatabase(DATABASE_NAME);
            carsCollection = database.getCollection(COLLECTION_NAME);
        } catch (Exception e) {
            System.err.println("Virhe MongoDB:hen yhdistämisessä: " + e.getMessage());
        }
    }

    public void close() {
        if (mongoClient != null) {
            mongoClient.close();
        }
    }

    /* CRUD-operaatiot */
    public List<Car> getAllCars() {
        List<Car> cars = new ArrayList<>();
        try {
            FindIterable<Document> docs = carsCollection.find();
            for (Document doc : docs) {
                Car car = documentToCar(doc);
                if (car != null) cars.add(car);
            }
        } catch (Exception e) {
            System.err.println("Virhe autojen noutamisessa: " + e.getMessage());
        }
        return cars;
    }

    public Car getCarById(ObjectId id) {
        try {
            Document doc = carsCollection.find(Filters.eq("_id")).first();
            if (doc != null) {
                return documentToCar(doc);
            }
        } catch (Exception e) {
            System.err.println("Virhe auton noutamisessa: " + e.getMessage());
        }
        return null;
    }

    public List<Car> searchCars(String manufacturer, String model, Integer yearFrom, Integer yearTo,
                                String engineType, Integer minPower, Integer maxPower) {
        List<Car> results = new ArrayList<>();
        List<Bson> filters = new ArrayList<>();

        // Lisätään suodattimet hakukriteerien perusteella
        if (manufacturer != null && !manufacturer.isEmpty()) {
            filters.add(Filters.regex("manufacturer", Pattern.compile(manufacturer, Pattern.CASE_INSENSITIVE)));
        }
        if (model != null && !model.isEmpty()) {
            filters.add(Filters.regex("model", Pattern.compile(model, Pattern.CASE_INSENSITIVE)));
        }
        if (yearFrom != null) {
            filters.add(Filters.gte("manufacturing_year", yearFrom));
        }
        if (yearTo != null) {
            filters.add(Filters.lte("manufacturing_year", yearTo));
        } 
        if (engineType != null && !engineType.isEmpty()) {
            filters.add(Filters.regex("engine", Pattern.compile(engineType, Pattern.CASE_INSENSITIVE)));
        }
        if (minPower != null) {
            filters.add(Filters.gte("power", minPower));
        }
        if (maxPower != null) {
            filters.add(Filters.lte("power", maxPower));
        }

        try {
            FindIterable<Document> docs;
            if (filters.isEmpty()) {
                docs = carsCollection.find();
            } else {
                docs = carsCollection.find(Filters.and(filters));
            }

            for (Document doc : docs) {
                results.add(documentToCar(doc));
            }
        } catch (Exception e) {
            System.err.println("Virhe autojen hakemisessa: " + e.getMessage());
        }

        return results;
    }

    public boolean addCar(Car car) {
        try {
            Document doc = new Document()
                .append("manufacturer", car.getManufacturer())
                .append("model", car.getModel())
                .append("distance_travelled", car.getDistanceTravelled())
                .append("manufacturing_year", car.getManufacturingYear())
                .append("engine", car.getEngine())
                .append("power", car.getPower());

            carsCollection.insertOne(doc);
            car.setId((ObjectId) doc.get("_id")); // Päivitetään car-olio luodulla id:llä
            return true;
        } catch (Exception e) {
            System.err.println("Virhe auton lisäämisessä: " + e.getMessage());
            return false;
        }
    }

    public boolean updateCar(Car car) {
        try {
            Bson filters = Filters.eq("_id", car.getId());
            Bson updates = Updates.combine(
                Updates.set("manufacturer", car.getManufacturer()),
                Updates.set("model", car.getModel()),
                Updates.set("distance_travelled", car.getDistanceTravelled()),
                Updates.set("manufacturing_year", car.getManufacturingYear()),
                Updates.set("engine", car.getEngine()),
                Updates.set("power", car.getPower())
            );

            return carsCollection.updateOne(filters, updates).getModifiedCount() > 0;
        } catch (Exception e) {
            System.err.println("Virhe auton päivittämisessä: " + e.getMessage());
            return false;
        }
    }

    public boolean deleteCar(ObjectId id) {
        try {
            Bson filter = Filters.eq("_id", id);
            return carsCollection.deleteOne(filter).getDeletedCount() > 0;
        } catch (Exception e) {
            System.err.println("Virhe auton poistamissa: " + e.getMessage());
            return false;
        }
    }

    // Apumetodi, joka muuntaa Document-tyypistä Car-objektiksi
    private Car documentToCar(Document doc) {
        Integer distance = doc.getInteger("distance_travelled");
        Integer year = doc.getInteger("manufacturing_year");
        Integer power = doc.getInteger("power");

        // Käsitellään null-arvot
        if (distance == null || year == null || power == null) {
            System.err.println("Ohitetaan auto (data puuttuu): " + doc.toJson());
            return null;
        }

        return new Car(
            doc.getObjectId("_id"),
            doc.getString("manufacturer"),
            doc.getString("model"),
            distance,
            year,
            doc.getString("engine"),
            power
        );
    }
}