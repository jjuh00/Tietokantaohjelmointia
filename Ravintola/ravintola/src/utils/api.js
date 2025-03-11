export const api = {
    // Haetaan kaikki kategoriat
    getCategories: async () => {
        try {
            const response = await fetch("http://localhost:3000/categories");
            const data = await response.json();

            // Muunnetaan kategoriat-olio taulukoksi
            return Object.keys(data).filter(c => c !== "mayonnaises");
        } catch (error) {
            console.log("Virhe kategorioita haettaessa: ", error);
            return [];
        }
    },

    // Haetaan käyttäjä ID:n perusteella
    getUserById: async (userId) => {
        try {
            const response = await fetch(`http://localhost:3000/users/${userId}`);
            return await response.json();
        } catch (error) {
            console.log("Virhe käyttäjän haussa: ", error);
            return null;
        }
    },

    // Päivitetään käyttäjä
    updateUser: async (userId, userData) => {
        try {
            const response = await fetch(`http://localhost:3000/users/${userId}`, {
                method: "PATCH",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify(userData)
            });

            return await response.json();
        } catch (error) {
            console.error("Virhe käyttäjän päivitykssessä: ", error);
            return null;
        }
    },

    // Haetaan tilaukset käyttäjälle
    getUserOrders: async (userId) => {
        try {
            const response = await fetch(`http://localhost:3000/orders?userId=${userId}`);
            return await response.json();
        } catch (error) {
            console.error("Virhe tilauksia haettaessa: ", error);
            return [];
        }
    },

    // Luodaan uusi tilaus
    createOrder: async (orderData) => {
        try {
            const response = await fetch(`http://localhost:3000/orders`, {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify(orderData)
            });

            return await response.json();
        } catch (error) {
            console.error("Virhe tilausta luodessa: ", error);
            return null;
        }
    }
};