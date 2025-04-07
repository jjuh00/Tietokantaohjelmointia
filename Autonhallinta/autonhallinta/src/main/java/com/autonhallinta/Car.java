package com.autonhallinta;

import org.bson.types.ObjectId;

public class Car {
    private ObjectId id;
    private String manufacturer;
    private String model;
    private int distanceTravelled;
    private int manufacturingYear;
    private String engine;
    private int power;

    // Oletus constructor
    public Car() {}

    // Kaikkien kenttien constructor (paitsi id:n) uuden auton luonti varten
    public Car(String manufacturer, String model, int distanceTravelled,
                int manufacturingYear, String engine, int power) {
        this.manufacturer = manufacturer;
        this.model = model;
        this.distanceTravelled = distanceTravelled;
        this.manufacturingYear = manufacturingYear;
        this.engine = engine;
        this.power = power;
    }

    // Kaikkien kenttien constructor (sisältäen id:n) tietokannasta lataamista varten
    public Car(ObjectId id, String manufacturer, String model, int distanceTravelled,
                int manufacturingYear, String engine, int power) {
        this.id = id;
        this.manufacturer = manufacturer;
        this.model = model;
        this.distanceTravelled = distanceTravelled;
        this.manufacturingYear = manufacturingYear;
        this.engine = engine;
        this.power = power;
    }

    // Getterit ja setterit
    public ObjectId getId() {
        return id;
    }

    public void setId(ObjectId id) {
        this.id = id;
    }

    public String getManufacturer() {
        return manufacturer;
    }

    public void setManufacturer(String manufacturer) {
        this.manufacturer = manufacturer;
    }

    public String getModel() {
        return model;
    }

    public void setModel(String model) {
        this.model = model;
    }

    public int getDistanceTravelled() {
        return distanceTravelled;
    }

    public void setDistanceTravelled(int distanceTravelled) {
        this.distanceTravelled = distanceTravelled;
    }

    public int getManufacturingYear() {
        return manufacturingYear;
    }

    public void setManufacturingYear(int manufacturingYear) {
        this.manufacturingYear = manufacturingYear;
    }

    public String getEngine() {
        return engine;
    }

    public void setEngine(String engine) {
        this.engine = engine;
    }

    public int getPower() {
        return power;
    }

    public void setPower(int power) {
        this.power = power;
    }

    @Override
    public String toString() {
        return manufacturer + " " + model + " (" + manufacturingYear + ")";
    }
}