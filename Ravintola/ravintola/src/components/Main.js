import React, { useState, useEffect} from 'react';
import { useNavigate } from 'react-router';
import '../styles/Main.css';

const Main = () => {
    const [categories, setCategories] = useState([]);
    const [currentUser, setCurrentUser] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        // Tarkistetaan, onko käyttäjä kirjautunut sisään
        const userStr = localStorage.getItem("currentUser");
        if (!userStr) {
            navigate("/");
            return;
        }

        setCurrentUser(JSON.parse(userStr));

        // Haetaan kategoriat
        fetchCategories();
    }, [navigate]);

    const fetchCategories = async () => {
        try {
            const response = await fetch("http://localhost:3000/categories");
            const data = await response.json();

            // Eristetään kategorioiden nimet datasta
            const categoryList = Object.keys(data).filter(c => c !== 'mayonnaises');
            setCategories(categoryList);
        } catch (error) {
            console.error("Virhe kategorioita haettaessa: ", error)
        }
    };

    const handleLogout = () => {
        localStorage.removeItem("currentUser");
        navigate("/");
    };

    return (
        <div className="main-container">
            <header>
                <h1>Ravintola</h1>
                {currentUser && (
                    <div className="user-info">
                        <span>Saldo: {currentUser.balance}€</span>
                        <button onClick={handleLogout} className="logout-btn">Kirjaudu ulos</button>
                    </div>
                )}
            </header>

            <nav className="navbar">
                <ul className="category-list">
                    {categories.map((category, i) => (
                        <li key={i} className="category-item">
                            {category === "pizzas" && "Pizzat"}
                            {category === "kebabs" && "Kebabit"}
                            {category === "chickens" && "Kanaruoat"}
                            {category === "salads" && "Salaatit"}
                            {category === "burgers" && "Burgerit"}
                            {category === "juomat" && "Juomat"}
                        </li>
                    ))}
                </ul>
            </nav>

            <main className="content">
                <p>Valitse kategoria</p>
            </main>
        </div>
    );
}

export default Main;