package com.autonhallinta;

import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.collections.ObservableList;
import javafx.collections.FXCollections;
import javafx.scene.control.*;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Modality;
import javafx.stage.Stage;
import org.bson.types.ObjectId;

import java.util.List;
import java.util.Arrays;
import java.io.IOException;
import java.util.Optional;

public class PrimaryController {
    
    @FXML
    private TableView<Car> carTableView;

    @FXML
    private TableColumn<Car, String> manufacturerColumn;

    @FXML
    private TableColumn<Car, String> modelColumn;

    @FXML
    private TableColumn<Car, Integer> yearColumn;
    
    @FXML
    private TableColumn<Car, Integer> distanceColumn;

    @FXML
    private TableColumn<Car, String> engineColumn;

    @FXML
    private TableColumn<Car, Integer> powerColumn;

    @FXML
    private TextField searchManufacturer;

    @FXML
    private TextField searchModel;

    @FXML
    private ComboBox<String> searchEngineType;

    @FXML
    private TextField searchYearFrom;

    @FXML
    private TextField searchYearTo;

    @FXML
    private TextField searchMinPower;

    @FXML
    private TextField searchMaxPower;

    private ObservableList<Car> carList = FXCollections.observableArrayList();
    private DatabaseService dbService = DatabaseService.getInstance();

    @FXML
    public void initialize() {
        // Asetetaan taulukon sarakkeet
        manufacturerColumn.setCellValueFactory(new PropertyValueFactory<>("manufacturer"));
        modelColumn.setCellValueFactory(new PropertyValueFactory<>("model"));
        yearColumn.setCellValueFactory(new PropertyValueFactory<>("manufacturingYear"));
        distanceColumn.setCellValueFactory(new PropertyValueFactory<>("distanceTravelled"));
        engineColumn.setCellValueFactory(new PropertyValueFactory<>("engine"));
        powerColumn.setCellValueFactory(new PropertyValueFactory<>("power"));

        // Asetetaan moottorityypin (käyttövoima) combobox
        List<String> engineTypes = Arrays.asList(
            "Kaikki", "Bensiini", "Diesel", "Sähkö", "Hybridi (Bensiini/Sähkö)", "Hybridi (Diesel/sähkö)"
        );
        searchEngineType.setItems(FXCollections.observableArrayList(engineTypes));
        searchEngineType.getSelectionModel().select("Kaikki");

        // Ladataan kaikki autot alustuksen yhteydessä
        loadAllCars();
    }

    private void loadAllCars() {
        carList.clear();
        carList.addAll(dbService.getAllCars());
        carTableView.setItems(carList);
    }

    @FXML
    private void handleSearch() {
        String manufacturer = searchManufacturer.getText().trim();
        String model = searchModel.getText().trim();

        Integer yearFrom = null;
        if (!searchYearFrom.getText().trim().isEmpty()) {
            try {
                yearFrom = Integer.parseInt(searchYearFrom.getText().trim());
            } catch (NumberFormatException e) {
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Vuoden pitää olla kokonaisluku");
                return;
            }
        }

        Integer yearTo = null;
        if (!searchYearTo.getText().trim().isEmpty()) {
            try {
                yearTo = Integer.parseInt(searchYearTo.getText().trim());
            } catch (NumberFormatException e) {
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Vuoden pitää olla kokonaisluku");
                return;
            }
        }

        String engineType = searchEngineType.getValue();
        if (engineType.equals("Kaikki")) {
            engineType = "";
        }

        Integer minPower = null;
        if (!searchMinPower.getText().trim().isEmpty()) {
            try {
                minPower = Integer.parseInt(searchMinPower.getText().trim());
            } catch (NumberFormatException e) {
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Tehon pitää olla kokonaisluku");
                return;
            }
        }

        Integer maxPower = null;
        if (!searchMaxPower.getText().trim().isEmpty()) {
            try {
                maxPower = Integer.parseInt(searchMaxPower.getText().trim());
            } catch (NumberFormatException e) {
                showAlert(Alert.AlertType.ERROR, "Epäkelpo syöte", "Tehon pitää olla kokonaisluku");
                return;
            }
        }

        // Haetaan kriteerien perusteella
        List<Car> results = dbService.searchCars(
            manufacturer.isEmpty() ? null : manufacturer,
            model.isEmpty() ? null : model,
            yearFrom,
            yearTo,
            engineType.isEmpty() ? null : engineType,
            minPower,
            maxPower
        );

        carList.clear();
        carList.addAll(results);
        carTableView.setItems(carList);
    }

    @FXML
    private void handleResetSearch() {
        searchManufacturer.clear();
        searchModel.clear();
        searchEngineType.getSelectionModel().select("Kaikki");
        searchYearFrom.clear();
        searchYearTo.clear();
        searchMinPower.clear();
        searchMaxPower.clear();

        loadAllCars();
    }

    @FXML
    private void handleAddCar() {
        try {
            FXMLLoader loader = new FXMLLoader(getClass().getResource("car_edit.fxml"));
            Parent root = loader.load();

            CarEditController controller = loader.getController();
            controller.setMode(CarEditController.Mode.ADD);

            Stage dialog = new Stage();
            dialog.initModality(Modality.APPLICATION_MODAL);
            dialog.setTitle("Lisää uusi auto");
            dialog.setScene(new Scene(root));

            dialog.showAndWait();

            // Ladataan autot uudelleen, kun dialogi suljetaan
            if (controller.isCarSaved()) {
                loadAllCars();
            }
        } catch (IOException e) {
            e.printStackTrace();
            showAlert(Alert.AlertType.ERROR, "Virhe", "Autonlisäys dialogia ei voitu avata");
        }
    }

    @FXML
    private void handleEditCar() {
        Car selectedCar = carTableView.getSelectionModel().getSelectedItem();
        if (selectedCar == null) {
            showAlert(Alert.AlertType.WARNING, "Ei valintaa", "Valitse muokattava auto");
            return;
        }

        try {
            FXMLLoader loader = new FXMLLoader(getClass().getResource("car_edit.fxml"));
            Parent root = loader.load();

            CarEditController controller = loader.getController();
            controller.setMode(CarEditController.Mode.EDIT);
            controller.setCar(selectedCar);

            Stage dialog = new Stage();
            dialog.initModality(Modality.APPLICATION_MODAL);
            dialog.setTitle("Muokkaa autoa");
            dialog.setScene(new Scene(root));

            dialog.showAndWait();

            // Ladataan autot uudelleen, kun dialogi suljetaan
            if (controller.isCarSaved()) {
                loadAllCars();
            }
        } catch (IOException e) {
            e.printStackTrace();
            showAlert(Alert.AlertType.ERROR, "Virhe", "Autonmuokkaus dialogia ei voitu avata");
        }
    }

    @FXML
    private void handleDeleteCar() {
        Car selectedCar = carTableView.getSelectionModel().getSelectedItem();
        if (selectedCar == null) {
            showAlert(Alert.AlertType.WARNING, "Ei valintaa", "Valitse poistettava auto");
            return;
        }

        Alert confirmDelete = new Alert(Alert.AlertType.CONFIRMATION);
        confirmDelete.setTitle("Vahvista poisto");
        confirmDelete.setHeaderText("Poista auto");
        confirmDelete.setContentText("Haluatko varmasti poistaa auton " + selectedCar.toString() + "?");

        Optional<ButtonType> result = confirmDelete.showAndWait();
        if (result.isPresent() && result.get() == ButtonType.OK) {
            ObjectId carId = selectedCar.getId();
            boolean success = dbService.deleteCar(carId);

            if (success) {
                carList.remove(selectedCar);
                showAlert(Alert.AlertType.INFORMATION, "Onnistui", "Auto poistettu onnistuneesti");
            } else {
                showAlert(Alert.AlertType.ERROR, "Virhe", "Auton poistaminen epäonnistui");
            }
        }
    }

    private void showAlert(Alert.AlertType type, String title, String message) {
        Alert alert = new Alert(type);
        alert.setTitle(title);
        alert.setHeaderText(null);
        alert.setContentText(message);
        alert.showAndWait();
    }
}