import mysql.connector # pip install mysql-connector-python
from datetime import datetime, timedelta

class Database:
    def __init__(self, host="localhost", user="root", password="root", database="LibraryDB"):
        # Alustetaan tietokantayhteys
        self.conn = mysql.connector.connect(host=host, user=user, password=password, database=database)
        self.cursor = self.conn.cursor(dictionary=True)

    def __del__(self):
        # Suljetaan tietokantayhteys, kun olio on tuhottu
        if hasattr(self, "conn") and self.conn.is_connected():
            self.cursor.close()
            self.conn.close()

    def add_book(self, title, author, isbn):
        # Lisätään uusi kirja tietokantaan
        query = "INSERT INTO books (title, author, isbn, available) VALUES (%s, %s, %s, 1)"
        self.cursor.execute(query, (title, author, isbn))
        self.conn.commit()
        return self.cursor.lastrowid
    
    def search_books(self, search_term):
        # Haetaan kirjaa sen nimellä, kirjailijalla tai ISBN:llä
        query = """
        SELECT * FROM books WHERE title LIKE %s OR author LIKE %s OR isbn LIKE %s"""
        search_pattern = f"%{search_term}%"
        self.cursor.execute(query, (search_pattern, search_pattern, search_pattern))
        return self.cursor.fetchall()
    
    def get_all_books(self):
        # Haetaan kaikki kirjat tietokannasta
        query = "SELECT * FROM books"
        self.cursor.execute(query)
        return self.cursor.fetchall()
    
    def update_book_availability(self, book_id, available):
        # Päivitetään kirjan saatavuutta
        query = "UPDATE books SET available = %s WHERE id = %s"
        self.cursor.execute(query, (available, book_id))
        self.conn.commit()

    def add_member(self, name, email, phone):
        # Lisätään uusi jäsen tietokantaan
        query = "INSERT INTO members (name, email, phone) VALUES (%s, %s, %s)"
        self.cursor.execute(query, (name, email, phone))
        self.conn.commit()
        return self.cursor.lastrowid
    
    def search_members(self, search_term):
        # Haetaan jäsen nimen, sähköpostin tai puhelinnumeron perusteella
        query = """
        SELECT * FROM members WHERE name LIKE %s OR email LIKE %s OR phone LIKE %s"""
        search_pattern = f"%{search_term}%"
        self.cursor.execute(query, (search_pattern, search_pattern, search_pattern))
        return self.cursor.fetchall()
    
    def borrow_book(self, book_id, member_id):
        # Lainataan kirja eli luodaan uusi lainatapahtuma
        # Tarkistetaan, onko kirja saatavilla
        check_query = "SELECT available FROM books WHERE id = %s"
        self.cursor.execute(check_query, (book_id,))
        result = self.cursor.fetchone()

        if not result or not result['available']:
            return False
        
        # Asetetaan kirja ei saatavilla olevaksi
        self.update_book_availability(book_id, 0)

        # Luodaan tapahtuma
        today = datetime.now().date()
        due_date = today + timedelta(days=30) # 30 päivän laina-aika

        query = """
        INSERT INTO transactions (book_id, member_id, borrow_date, due_date)
        VALUES (%s, %s, %s, %s)"""
        self.cursor.execute(query, (book_id, member_id, today, due_date))
        self.conn.commit()
        return True
    
    def return_book(self, book_id):
        # Merkataan kirja palautetuksi
        # Päivitetään kirjan saatavuus
        self.update_book_availability(book_id, 1)

        # Päivitetään tapahtuma
        query = """
        UPDATE transactions SET return_date %s WHERE book_id = %s AND return_date IS NULL"""
        today = datetime.now().date()
        self.cursor.execute(query, (today, book_id))
        self.conn.commit()
        return self.cursor.rowcount > 0
    
    def get_active_loans(self):
        # Haetaan kaikki lainat (kirjan ja jäsenen tiedoilla)
        query = """
        SELECT t.id, t.borrow_date, t.due_date,
               b.id as book_id, b.title, b.author, b.isbn,
               m.id as member_id, m.name, m.email, m.phone
        FROM transactions t
        JOIN books b ON t.book_id = b.id
        JOIN members m ON t.member_id = m.id
        WHERE t.return_date IS NULL
        """
        self.cursor.execute(query)
        return self.cursor.fetchall()