export const authentication = {
    // Salataan data yksinkertaisella base64-salauksella
    encrypt: (data) => {
        return btoa(data);
    },

    // Puretaan salattu data
    decrypt: (data) => {
        return atob(data);
    },

    // Käyttäjän sisään kirjautuminen
    login: async (email, password) => {
        try {
            const encryptedEmail = authentication.encrypt(email);
            const encryptedPassword = authentication.encrypt(password);

            const response = await fetch("http://localhost:3000/users");
            const users = await response.json();

            const user = users.find(
                (user) => user.email === encryptedEmail && user.password === encryptedPassword
            );

            if (user) {
                // Tallennetaan käyttäjätiedot localStoragen avulla (yksinkertaistettu käyttäjän tunnistus)
                const userData = {
                    id: user.id,
                    email: email,
                    balance: user.balance
                };

                localStorage.setItem("currentUser", JSON.stringify(userData));
                return { success: true, user: userData };
            } else {
                return { success: false, error: "Väärä sähköposti tai salasana"}
            }
        } catch (error) {
            console.error("Kirjautumisvirhe: ", error);
            return { success: false, error: "Kirjautuminen epäonnistui"}
        }
    },

    // Käyttäjän rekisteröinti
    register: async (email, password, balance) => {
        try {
            const encryptedEmail = authentication.encrypt(email);
            const encryptedPassword = authentication.encrypt(password);

            // Tarkistetaan, onko käyttäjä jo olemassa
            const usersResponse = await fetch("http://localhost:3000/users");
            const users = await usersResponse.json();

            const userExists = users.some(user => user.email === encryptedEmail);

            if (userExists) {
                return { success: false, error: "Sähköposti on jo käytössä" };
            }

            // Luodaan uusi käyttäjä
            const newUser = {
                email: encryptedEmail,
                password: encryptedPassword,
                balance: parseFloat(balance)
            };

            // Lisätään uusi käyttäjä tieotkantaan
            const response = await fetch("http://localhost:3000/users", {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify(newUser)
            });

            if (response.ok) {
                const createdUser = await response.json();

                // Tallennetaan käyttäjätiedot localStoragen avlla
                const userData = {
                    id: createdUser.id,
                    email: email,
                    balance: createdUser.balance
                };

                localStorage.setItem("currentUser", JSON.stringify(userData));
                return { success: true, user: userData }
            } else {
                return { success: false, error: "Rekisteröinti epäonnistui" };
            }
        } catch (error) {
            console.error("Rekisteröintivirhe: ", error);
            return { success: false, error: "Rekisteröinti epäonnistu"}
        }
    },

    // Käyttäjän ulos kirjautuminen
    logout: () => {
        localStorage.removeItem("currentUser");
    },
    
    // Tarkistetaan, onko käyttäjä kirjautunut sisään
    isLoggedIn: () => {
        return localStorage.getItem("currentItem") !== null;
    },

    // Haetaan nykyinen käyttäjä
    getCurrentUser: () => {
        const userStr = localStorage.getItem("currentUser");
        if (!userStr) return null;

        return JSON.parse(userStr);
    }
};