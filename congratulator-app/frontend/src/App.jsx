import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
    const [birthdays, setBirthdays] = useState([]);
    const [showForm, setShowForm] = useState(false);
    const [editingPerson, setEditingPerson] = useState(null);
    const [newPerson, setNewPerson] = useState({
        firstName: '',
        lastName: '',
        birthDate: '',
        email: '',
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

    const openEditForm = (person) => {
        setEditingPerson(person);
        setNewPerson({
            firstName: person.firstName,
            lastName: person.lastName,
            birthDate: person.birthDate,
            email: person.email,
            photoFile: null
        });
        setShowForm(true);
    };

    const SavePerson = async () => {
        if (!newPerson.firstName || !newPerson.lastName || !newPerson.birthDate || !newPerson.email) {
            alert('Заполните все обязательные поля (Имя, Фамилия, Дата рождения, Email)');
            return;
        }

        const formData = new FormData();
        formData.append('firstName', newPerson.firstName);
        formData.append('lastName', newPerson.lastName);
        formData.append('birthDate', newPerson.birthDate);
        formData.append('email', newPerson.email);
        if (newPerson.photoFile) {
            formData.append('photo', newPerson.photoFile);
        }

        try {
            let response;
            if (editingPerson) {
                response = await axios.put(`http://localhost:5227/api/Birthdays/${editingPerson.id}`, formData, {
                    headers: { 'Content-Type': 'multipart/form-data' },
                });
                alert('Именинник обновлён');
            } else {
                response = await axios.post('http://localhost:5227/api/Birthdays', formData, {
                    headers: { 'Content-Type': 'multipart/form-data' },
                });
                alert('Именинник добавлен');
            }

            console.log('Сохранён:', response.data);

            setShowForm(false);
            setEditingPerson(null);
            setNewPerson({ firstName: '', lastName: '', birthDate: '', email: '', photoFile: null });

            await loadAll();
        } catch (error) {
            console.error('Ошибка сохранения:', error);
            if (error.response?.data?.errors) {
                alert('Ошибки:\n' + error.response.data.errors.join('\n'));
            } else {
                alert('Не удалось сохранить именинника');
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
                    <button className="all" onClick={loadAll}>Все</button>
                    <button className="nearest" onClick={loadUpcoming}>Ближайшие</button>
                    <button className="add" onClick={() => setShowForm(true)}>Добавить</button>
                </div>

                <div className={`overlay ${showForm ? 'active' : ''}`} onClick={() => { setShowForm(false); setEditingPerson(null); }}></div>

                <div className={`AddWindow ${showForm ? 'visible' : ''}`}>
                    <h2>{editingPerson ? 'Редактировать именинника' : 'Добавить именинника'}</h2>
                    <input
                        type="text"
                        placeholder="Имя"
                        value={newPerson.firstName}
                        onChange={(e) => setNewPerson({ ...newPerson, firstName: e.target.value })}
                    />
                    <input
                        type="text"
                        placeholder="Фамилия"
                        value={newPerson.lastName}
                        onChange={(e) => setNewPerson({ ...newPerson, lastName: e.target.value })}
                    />
                    <input
                        type="date"
                        placeholder="Дата рождения"
                        value={newPerson.birthDate}
                        onChange={(e) => setNewPerson({ ...newPerson, birthDate: e.target.value })}
                    />
                    <input
                        type="email"
                        placeholder="Email"
                        value={newPerson.email}
                        onChange={(e) => setNewPerson({ ...newPerson, email: e.target.value })}
                    />
                    <input
                        type="file"
                        accept="image/*"
                        onChange={(e) => setNewPerson({ ...newPerson, photoFile: e.target.files[0] })}
                    />
                    <div className="form-buttons">
                        <button className="save-btn" onClick={SavePerson}>Сохранить</button>
                        <button className="cancel-btn" onClick={() => { setShowForm(false); setEditingPerson(null); }}>Отмена</button>
                    </div>
                </div>

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
                                <div className="edit" onClick={() => openEditForm(person)}>Редактировать</div>
                                <div className="delete" onClick={() => DeletePerson(person.id)}>Удалить</div>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        </>
    );
}

export default App;