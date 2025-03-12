import React, { useState, useEffect } from 'react';
import './App.css';

const URL = "http://localhost:3004/tasks";

const App = () => {
    const [tasks, setTasks] = useState([]);
    const [newTask, setNewTask] = useState("");
    const [status, setStatus] = useState({ message: "", type: "" });

    useEffect(() => {
        fetchTasks();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const fetchTasks = async () => {
        try {
            const response = await fetch(URL);
            if (!response.ok) throw new Error("Haku epäonnistui");
            const data = await response.json();
            // Suodatetaan pois suoritetut tehtävät
            const activeTasks = data.filter(task => !task.completed);
            setTasks(activeTasks)
        } catch (error) {
            showStatus("Tehtävien haku epäonnistui", "virhe");
        }
    };

    const addTask = async (e) => {
        e.preventDefault();

        if (!newTask.trim()) {
            showStatus("Tehtävä ei voi olla tyhjä", "virhe");
            return;
        }

        // Lisätään uusi tehtävä tietokantaan
        try {
            const response = await fetch(URL, {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify({
                    title: newTask,
                    completed: false,
                    dateAdded: new Date().toISOString(),
                    dateCompleted: null
                })
            });

            if (!response.ok) throw new Error("Tehtävän lisääminen epäonnistui");
            const data = await response.json();

            setTasks([...tasks, data]);
            setNewTask("");
            showStatus("Tehtävä lisätty onnistuneesti", "success");
        } catch (error) {
            showStatus("Tehtävän lisääminen epäonnistui", "error");
        }
    };

    const completeTask = async (id) => {
        // Muokataan ja merkitään tehtävä tehdykdi tietokantaan
        try {
            const response = await fetch(`${URL}/${id}`, {
                method: "PATCH",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify({completed: true, dateCompleted: new Date().toISOString()})
            });

            if (!response.ok) throw new Error("Tehtävän suorittaminen epäonnistui");

            // Suodatetaan tehty tehtävä pois tehtävistä
            setTasks(tasks.filter(task => task.id !== id));
            showStatus("Tehtävä suoritettu onnistuneest", "success");
        } catch (error) {
            showStatus("Tehtävän suorittaminen epäonnistui", "error");
        }
    };

    const deleteTask = async (id) => {
        // Poistetaan tehtävä tietokannasta
        try {
            const response = await fetch(`${URL}/${id}`, {
                method: "DELETE"
            });

            if (!response.ok) throw new Error("Tehtävän poistaminen epäonnistui");

            // Suodatetaan poistettu tehtävä
            setTasks(tasks.filter(task => task.id !== id));
            showStatus("Tehtävä poistettu onnistuneesti", "success");
        } catch (error) {
            showStatus("Tehtävän poistaminen epäonnistui", "error");
        }
    }

    const showStatus = (message, type) => {
        setStatus({ message, type });
        setTimeout(() => {
            setStatus({ message: "", type: "" });
        }, 4000)
    };

    const formatDate = (dateStr) => {
        const date = new Date(dateStr);
        return date.toLocaleDateString('fi-FI', {
            year: "numeric",
            month: "short",
            day: "numeric"
        });
    };

    return (
        <div className="container">
            <div className="app">
                <h1 className="title">Tehtävälista</h1>

                <form className="todo-form" onSubmit={addTask}>
                    <input 
                        type="text"
                        className="todo-input"
                        placeholder="Lisää uusi tehtävä"
                        value={newTask}
                        onChange={(e) => setNewTask(e.target.value)}
                    />
                    <button type="submit" className="add-btn">Lisää</button>
                </form>

                {tasks.length > 0 ? (
                    <ul className="todo-list">
                        {tasks.map(task => (
                            <li key={task.id} className="todo-item">
                                <div className="todo-text">
                                    {task.title}
                                    <span className="date-added">Lisätty: {formatDate(task.dateAdded)}</span>
                                </div>
                                <div className="action-buttons">
                                    <button 
                                        className="complete-btn"
                                        onClick={() => completeTask(task.id)}
                                        title="Merkitse tehdyksi"
                                    >
                                        ✓
                                    </button>
                                    <button
                                        className="delete-btn"
                                        onClick={() => deleteTask(task.id)}
                                        title="Poista tehtävä"
                                    >
                                        x
                                    </button>
                                </div>
                            </li>
                        ))}
                    </ul>
                ) : (
                    <div className="empty-list">Ei näytettäviä tehtäviä</div>
                )}

                {status.message && (
                    <div className={`status-messsage staus-${status.type}`}>
                        {status.message}
                    </div>
                )}
            </div>
        </div>
    );
}

export default App;