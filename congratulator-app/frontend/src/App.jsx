import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App(){
    const [birthdays, setBirthdays] = useState([]);
    
    useEffect(() => {
        loadAll();
    }, []);

    const loadUpcoming = async () => {
        try {
            const response = await axios.get('http://localhost:5227/api/Birthdays/upcoming?days=7');
            setBirthdays(response.data);
        } catch (error) {
            console.error('Ошибка загрузки ближайших:', error);
        }
    };

    const loadAll = async () => {
        try{
            const response = await axios.get('http://localhost:5227/api/Birthdays');
            setBirthdays(response.data);
        } catch (error) {
            console.error('Ошибка:', error);
        }
    };

    const DeletePerson = async (id) => {
        if (!window.confirm('🗑️ Удалить этого именинника?')) return;
        try{
            await axios.delete(`http://localhost:5227/api/Birthdays/${id}`);

            await loadAll();
            alert('Именинник удалён');
        } catch (error) {
            console.error('Ошибка удаления:', error);
            alert('Не удалось удалить');
        }
    }

    return (
        <>
            <header>
                <div className="boxzag">
                    <h1>Поздравлятор</h1>
                </div>
            </header>
            <main>
                <div className="navigation">
                    <button className="all">Все</button>
                    <button className="nearest">Ближайшие</button>
                    <button className="add">Добавить</button>
                </div>
                <div className="cards">
                {birthdays.map(person => (
                    <div className="card" key={person.id}>
                    <div className="photo">
                    {person.photoPath ? (
                        <img className="photo-img"
                        src={`http://localhost:5227${person.photoPath}`} 
                        alt="Фото"
                        />
                    ) : (
                        <span style={{ fontSize: '40px' }}>👤</span>
                    )}
                    </div>
                    <div className="nameuser">{person.firstName} {person.lastName}</div>
                    <div className="datebirth">📅 {person.birthDate}</div>
                    <div className="editDeleteBox">
                        <div className="edit">✏️</div>
                        <div className="delete">🗑️</div>
                    </div>
                    </div>
                ))}
                </div>
            </main>
        </>
    )
}

export default App;