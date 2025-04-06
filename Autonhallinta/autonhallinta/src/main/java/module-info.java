module com.autonhallinta {
    requires javafx.controls;
    requires javafx.fxml;

    opens com.autonhallinta to javafx.fxml;
    exports com.autonhallinta;
}
