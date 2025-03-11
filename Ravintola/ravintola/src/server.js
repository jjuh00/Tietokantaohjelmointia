const { spawn } = require("child_process");

// Määritetään polku JSON-tietokantaan
const polku = "db.json";

// Käynnistetään json-server
const jsonServer = spawn("npx", ["json-server", "--watch", polku, "--port", "3000"], {
    stdio: "inherit",
    shell: true // Varmistetaan, että toimii eri alustoilla
});

// Sammutetaan json-server
jsonServer.on("exit", (code) => {
    console.log(`json-server exited with code ${code}`);
});