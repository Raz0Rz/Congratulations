import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
    const [birthdays, setBirthdays] = useState([]);
    const [showForm, setShowForm] = useState(false);
    const [newPerson, setNewPerson] = useState({
        firstName: '',
        lastName: '',
        birthDate: '',
        photoFile: null
    });

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
        try {
            const response = await axios.get('http://localhost:5227/api/Birthdays');
            setBirthdays(response.data);
        } catch (error) {
            console.error('Ошибка загрузки всех:', error);
        }
    };

    const DeletePerson = async (id) => {
        if (!window.confirm('Удалить этого именинника?')) return;
        try {
            await axios.delete(`http://localhost:5227/api/Birthdays/${id}`);
            await loadAll();
            alert('Именинник удалён');
        } catch (error) {
            console.error('Ошибка удаления:', error);
            alert('Не удалось удалить');
        }
    };

    const CreatePerson = async () => {
        if (!newPerson.firstName || !newPerson.lastName || !newPerson.birthDate) {
            alert('Заполните все обязательные поля (Имя, Фамилия, Дата рождения)');
            return;
        }

        const formData = new FormData();
        formData.append('firstName', newPerson.firstName);
        formData.append('lastName', newPerson.lastName);
        formData.append('birthDate', newPerson.birthDate);
        if (newPerson.photoFile) {
            formData.append('photo', newPerson.photoFile);
        }

        try {
            const response = await axios.post('http://localhost:5227/api/Birthdays', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
            console.log('Добавлен:', response.data);

            setShowForm(false);
            setNewPerson({ firstName: '', lastName: '', birthDate: '', photoFile: null });

            await loadAll();
            alert('Именинник добавлен');
        } catch (error) {
            console.error('Ошибка добавления:', error);
            if (error.response?.data?.errors) {
                alert('Ошибки:\n' + error.response.data.errors.join('\n'));
            } else {
                alert('Не удалось добавить именинника');
            }
        }
    };

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

                {showForm && (
                    <div className="form-modal">
                        <h2>Добавить именинника</h2>
                        <input
                            type="text"
                            placeholder="Имя *"
                            value={newPerson.firstName}
                            onChange={(e) => setNewPerson({ ...newPerson, firstName: e.target.value })}
                        />
                        <input
                            type="text"
                            placeholder="Фамилия *"
                            value={newPerson.lastName}
                            onChange={(e) => setNewPerson({ ...newPerson, lastName: e.target.value })}
                        />
                        <input
                            type="date"
                            placeholder="Дата рождения *"
                            value={newPerson.birthDate}
                            onChange={(e) => setNewPerson({ ...newPerson, birthDate: e.target.value })}
                        />
                        <input
                            type="file"
                            accept="image/*"
                            onChange={(e) => setNewPerson({ ...newPerson, photoFile: e.target.files[0] })}
                        />
                        <div className="form-buttons">
                            <button className="save-btn">Сохранить</button>
                            <button className="cancel-btn">Отмена</button>
                        </div>
                    </div>
                )}

                <div className="cards">
                    {birthdays.map(person => (
                        <div className="card" key={person.id}>
                            <div className="photo">
                                {person.photoPath ? (
                                    <img
                                        className="photo-img"
                                        src={`http://localhost:5227${person.photoPath}`}
                                        alt="Фото"
                                    />
                                ) : (
                                    <span style={{ fontSize: '40px' }}>👤</span>
                                )}
                            </div>
                            <div className="nameuser">{person.firstName} {person.lastName}</div>
                            <div className="datebirth">{person.birthDate}</div>
                            <div className="editDeleteBox">
                                <div className="edit">✏️</div>
                                <div className="delete">🗑️</div>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        </>
    );
}

export default App;