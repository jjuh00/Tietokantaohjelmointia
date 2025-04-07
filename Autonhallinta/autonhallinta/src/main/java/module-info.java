module com.autonhallinta {
    requires javafx.controls;
    requires javafx.fxml;
    requires org.mongodb.driver.sync.client;
    requires org.mongodb.bson;
    requires org.mongodb.driver.core;

    opens com.autonhallinta to javafx.fxml;
    exports com.autonhallinta;
}