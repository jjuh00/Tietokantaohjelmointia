const { spawn } = require("child_process");

// Määritetään polku tietokantaan
const path = "db.json";

// Käynnistetään json-server
const jsonServer = spawn("npx", ["json-server", "--watch", path, "--port", "3004"], {
    stdio: "inherit",
    shell: true // Varmistaa toimimisen eri alustoilla
});

jsonServer.on("exit", (code) => {
    console.log(`json-server exited with code ${code}`)
});