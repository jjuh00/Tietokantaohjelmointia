package com.autonhallinta;

import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.collections.FXCollections;
import javafx.stage.Stage;

import java.util.List;
import java.util.Arrays;
import java.time.Year;

public class CarEditController {

    public enum Mode {
        ADD,
        EDIT
    }

    @FXML
    private Label titleLabel;

    @FXML
    private TextField manufacturerField;

    @FXML
    private TextField modelField;

    @FXML
    private TextField yearField;

    @FXML
    private TextField distanceField;

    @FXML
    private ComboBox<String> engineTypeCombo;

    @FXML
    private TextField powerField;

    private Mode currentMode = Mode.ADD;
    private Car currentCar;
    private boolean carSaved = false;
    private DatabaseService dbService = DatabaseService.getInstance();

    @FXML
    public void initialize() {
        // Asetetaan moottorityypin (käyttövoima) combobox
        List<String> engineTypes = Arrays.asList(
            "Bensiini", "Diesel", "Sähkö", "Hybridi (Bensiini/Sähkö)", "Hybridi (Diesel/Sähkö)"
        );
        engineTypeCombo.setItems(FXCollections.observableArrayList(engineTypes));
        engineTypeCombo.getSelectionModel().select(0);

        // Asetetaan syötteen validointi numeerisille kentille
        setNumericValidation(yearField);
        setNumericValidation(distanceField);
        setNumericValidation(powerField);
    }

    public void setMode(Mode mode) {
        this.currentMode = mode;
        if (mode == Mode.ADD) {
            titleLabel.setText("Lisää uusi auto");
        } else {
            titleLabel.setText("Muokkaa autoa");
        }
    }

    public void setCar(Car car) {
        this.currentCar = car;

        if (car != null) {
            manufacturerField.setText(car.getManufacturer());
            modelField.setText(car.getModel());
            yearField.setText(String.valueOf(car.getManufacturingYear()));
            distanceField.setText(String.valueOf(car.getDistanceTravelled()));
            
            // Valitaan vastaava moottorityyppi (käyttövoima) comboboxissa
            for (int i = 0; i < engineTypeCombo.getItems().size(); i++) {
                if (engineTypeCombo.getItems().get(i).equals(car.getEngine())) {
                    engineTypeCombo.getSelectionModel().select(i);
                    break;
                }
            }

            powerField.setText(String.valueOf(car.getPower()));
        }
    }

    public boolean isCarSaved() {
        return carSaved;
    }

    @FXML
    private void handleSave() {
        if (!validateInputs()) return;

        String manufacturer = manufacturerField.getText().trim();
        String model = modelField.getText().trim();
        int year = Integer.parseInt(yearField.getText().trim());
        int distance = Integer.parseInt(distanceField.getText().trim());
        String engine = engineTypeCombo.getValue();
        int power = Integer.parseInt(powerField.getText().trim());

        boolean success;
        if (currentMode == Mode.ADD) {
            Car newCar = new Car(manufacturer, model, distance, year, engine, power);
            success = dbService.addCar(newCar);
        } else {
            currentCar.setManufacturer(manufacturer);
            currentCar.setModel(model);
            currentCar.setManufacturingYear(year);
            currentCar.setDistanceTravelled(distance);
            currentCar.setEngine(engine);
            currentCar.setPower(power);
            success = dbService.updateCar(currentCar);
        }

        if (success) {
            carSaved = true;
            closeDialog();
        } else {
            showAlert(Alert.AlertType.ERROR, "Virhe", "Auton tietojen tallentaminen epäonnistui");
        }
    }

    @FXML
    private void handleCancel() {
        closeDialog();
    }

    private boolean validateInputs() {
        // Tarkistus tyhjien kenttien varalta
        if (manufacturerField.getText().trim().isEmpty()) {
            showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Valmistaja ei voi olla tyhjä");
            return false;
        }

        if (modelField.getText().trim().isEmpty()) {
            showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Malli ei voi olla tyhhjä");
            return false;
        }

        // Tarkistetaan numeeriset kentät
        try {
            int year = Integer.parseInt(yearField.getText().trim());
            if (year < 1900 || year > Year.now().getValue() + 1) { // Vuoden pitää olla 1900 ja seuraavan vuoden (tällä hetkellä 2025 + 1) välillä
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Vuoden pitää olla 1900 ja ensi vuoden välillä");
                return false;
            } 
        } catch (NumberFormatException e) {
            showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Vuoden pitää olla kokonaisluku");
        }

        try {
            int distance = Integer.parseInt(distanceField.getText().trim());
            if (distance < 0 || distance > 10000000) {
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Mittarilukeman pitää olla välillä 0 km - 10 000 000 km");
                return false;
            }   
        } catch (NumberFormatException e) {
            showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Mittarilukeman pitää olla kokonaisluku");
            return false;
        }

        try {
            int power = Integer.parseInt(powerField.getText().trim());
            if (power <= 0 || power > 9999) {
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Tehon pitää olla 1 ja 9999 välillä");
                return false;
            } 
        } catch (NumberFormatException e) {
            showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Tehon pitää olla kokonaisluku");
            return false;
        }

        return true;
    }

    private void setNumericValidation(TextField field) {
        field.textProperty().addListener((observable, oldValue, newValue) -> {
            if (!newValue.matches("\\d*")) {
                field.setText(newValue.replaceAll("[^\\d]", ""));
            }
        });
    }

    private void closeDialog() {
        Stage stage = (Stage) manufacturerField.getScene().getWindow();
        stage.close();
    }

    private void showAlert(Alert.AlertType type, String title, String message) {
        Alert alert = new Alert(type);
        alert.setTitle(title);
        alert.setHeaderText(null);
        alert.setContentText(message);
        alert.showAndWait();
    }
}