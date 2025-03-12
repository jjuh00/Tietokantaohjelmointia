import React, { useState, useEffect} from 'react';
import { useNavigate } from 'react-router';
import { api } from '../utils/api';
import { authentication } from '../utils/authentication';
import '../styles/Main.css';

const Main = () => {
    const [categories, setCategories] = useState([]);
    const [currentUser, setCurrentUser] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        // Tarkistetaan, onko käyttäjä kirjautunut sisään
        const user = authentication.getItem("currentUser");
        if (!user) {
            navigate("/");
            return;
        }

        setCurrentUser(user);

        // Haetaan kategoriat
        fetchCategories();
    }, [navigate]);

    const fetchCategories = async () => {
        try {
            const categories = await api.getCategories();
            setCategories(categories);
        } catch (error) {
            console.error("Virhe kategorioita haettaessa: ", error)
        }
    };

    const handleLogout = () => {
        authentication.logout();
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