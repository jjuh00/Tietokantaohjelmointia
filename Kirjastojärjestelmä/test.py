import unittest
import sys
from unittest.mock import patch, MagicMock
from datetime import datetime, timedelta

sys.path.append('.')

# Importataan tarvittavat luokat testausta varten
from database import Database
from book_management import BookManager
from member_management import MemberManager
from transaction_management import TransactionManager

class LibrarySystemTest(unittest.TestCase):
    # Testataan mm. tietokantaoperaatioita, kirjojen ja jäsenten hallintaa sekä tapahtumien käsittelyä

    def setUp(self):
        """Valmistellaan testiympäristö ennen jokaista testiä"""
        # Luodaan testausta varten jäljitelmä (mock) tietokantayhteydelle
        self.mock_conn = MagicMock()
        self.mock_cursor = MagicMock()

        # Jäljitellään mysql.connector.connect metodia ylempämä luodun jäljitelmäyhteyden palauttamiseksi
        self.patcher = patch("mysql.connector.connect")
        self.mock_connect = self.patcher.start()
        self.mock_connect.return_value = self.mock_conn
        self.mock_conn.cursor.return_value = self.mock_cursor

        # Luodaan tietokantainstanssi jäljitelmäyhteydelle
        self.db = Database(host="localhost", user="test", password="test", database="TestDatabase")

        # Luodaan jäljitelmät käyttöliittymäkomponentteja varten
        self.mock_parent = MagicMock()
        self.mock_app = MagicMock()

    def tearDown(self):
        """Siivotaan jokaisen testin jälkeen"""
        self.patcher.stop()

    def test_database_connection(self):
        """Testataan tietokantayhteyden alustamista"""
        # Tarkistetaan, yritettiinkö yhteys muodostaa oikeilla parametreilla
        self.mock_connect.assert_called_once_with(host="localhost", user="test", password="test", database="TestDatabase")

    def test_add_book(self):
        """Testataan kirjan lisäämistä tietokantaan"""
        # Odotettu lastrowid:n palauttama arvo
        self.mock_cursor.lastrowid = 1

        book_id = self.db.add_book("Python testaajan käsikirja", "Testaaja", "354695464534")

        # Tarkistetaan, suorittiko funktio oikean SQL-kyselyn
        self.mock_cursor.execute.assert_called_with(
            "INSERT INTO books (title, author, isbn, available) VALUES (%s, %s, %s, 1)",
            ("Python testaajan käsikirja", "Testaaja", "354695464534")
        )

        self.mock_conn.commit.assert_called_once()

        # Tarkistetaan, palautettiinko oikea book_id
        self.assertEqual(book_id, 1)

    def test_search_books(self):
        """Testataan kirjojen hakemista"""
        # Jäljitellään fetchall-metodia testidatan palautusta varten
        sample_book = [{"id": 1, "title": "Python testaajan käsikirja", "author": "Testaaja", "isbn": "354695464534", "available": 1}]
        self.mock_cursor.fetchall.return_value = sample_book

        result = self.db.search_books("Python")

        # Tarkistetaan, suorittiko funktio oikean SQL-kyselyn
        self.mock_cursor.execute.assert_called()

        # Tarkistetaan, palautettiinko oikea data
        self.assertEqual(result, sample_book)

    def test_add_member(self):
        """Testataan jäsenen lisäämistä tietokantaan"""
        # Odotettu lastrowid:n palauttama arvo
        self.mock_cursor.lastrowid = 1

        member_id = self.db.add_member("Testaaja", "testi@esimerkki.fi", "0401234567")

        # Testataan, suorittiko funktio oikean SQL-kyselyn
        self.mock_cursor.execute.assert_called_with(
            "INSERT INTO members (name, email, phone) VALUES (%s, %s, %s)",
            ("Testaaja", "testi@esimerkki.fi", "0401234567")
        )

        self.mock_conn.commit.assert_called_once()

        # Tarkistetaan, palautettinko oikea member_id
        self.assertEqual(member_id, 1)

    def test_borrow_book_success(self):
        """Testataan onnistunutta kirjan lainaamista"""
        self.mock_cursor.fetchone.return_value = {"available": 1}
        result = self.db.borrow_book(1, 1)

        # Tarkistetaan, suorittiko funktio oikean SQL-kyselyn ja että päivittykö kirjan saatavuus
        self.mock_cursor.execute.assert_any_call("SELECT available FROM books WHERE id = %s", (1,))
        self.mock_cursor.execute.assert_any_call("UPDATE books SET available = %s WHERE id = %s", (0, 1))

        # Tarkistetaan, suoritettiinko tapahtumalle INSERT-kysely
        self.assertTrue(self.mock_cursor.execute.call_count >= 3)

        self.mock_conn.commit.assert_called()

        self.assertTrue(result)

    def test_borrow_book_unavailable(self):
        """Testataan ei saatavilla olevan kirjan lainaamista"""
        # Odotettu palautusarvo kirjan saatavuudelle (0 = ei saatavilla)
        self.mock_cursor.fetchone.return_value = {"available": 0}

        result = self.db.borrow_book(1, 1)

        # Tarkistetaan, suorittiko funktio oikean SQL-kyselyn
        self.mock_cursor.execute.assert_called_once_with("SELECT available FROM books WHERE id = %s", (1,))

        # Tarkistetaan, palautettiinko oikea arvo
        self.assertFalse(result)

    def test_return_book(self):
        """Testataan kirjan palautus"""
        self.mock_cursor.rowcount = 1
        result = self.db.return_book(1)

        # Tarkistetaan, päivitettiinkö kirjan saatavuus
        self.mock_cursor.execute.assert_any_call("UPDATE books SET available = %s WHERE id = %s", (1, 1))

        # Tarkistetaan, päivitettiinkö palautustapahtuma
        self.assertTrue(self.mock_cursor.execute.call_count == 2)
        
        self.mock_conn.commit.assert_called()

        # Tarkistetaa, palautettiinko oikea arvo
        self.assertTrue(result)

    def test_get_active_loans(self):
        """Testataan aktiivisten lainojen hakemista"""
        # Testidata aktiivisille lainoille
        sample_date = datetime.now().date()
        sample_loan = [
            {
                "id": 1,
                "book_id": 1,
                "member_id": 1,
                "borrow_date": sample_date,
                "due_date": sample_date + timedelta(days=30),
                "title": "Python testaajan käsikirja",
                "author": "Testaaja",
                "isbn": "354695464534",
                "name": "Testaaja",
                "email": "testi@esimerkki.fi",
                "phone": "0401234567"
            }
        ]
        self.mock_cursor.fetchall.return_value = sample_loan

        result = self.db.get_active_loans()

        # Testataan, suorittiko funktio oikean SQL-kyselyn
        self.mock_cursor.execute.assert_called_once()

        # Tarkistetaan, palautettiinko oikea data
        self.assertEqual(result, sample_loan)

    def test_update_book_availability(self):
        """Testataan kirjan saatavuuden päivitystä"""
        self.db.update_book_availability(1, 0)

        # Tarkistetaan, suorittiko funktio oikean SQL-kyselyn
        self.mock_cursor.execute.assert_called_with(
            "UPDATE books SET available = %s WHERE id = %s",
            (0, 1)
        )

        self.mock_conn.commit.assert_called_once()

    def test_book_manager_initialization(self):
        """Testatataan, että BookManagerin alustus luo tarvittavat käyttöliittymäelementit"""
        # Luodaan jäljitelmillä BookManager-instanssi
        with patch("tkinter.ttk.LabelFrame"), \
             patch("tkinter.ttk.Entry"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Button"), \
             patch("tkinter.ttk.Treeview"), \
             patch("tkinter.ttk.Scrollbar"):
            
            book_manager = BookManager(self.mock_parent, self.db, self.mock_app)

            # Tarkistetaan, luotiinko kehys
            self.assertIsNotNone(book_manager.frame)

            # Tarkistetaan, luotiinko puunäkymä
            self.assertIsNotNone(book_manager.tree)

    def test_member_manager_initialization(self):
        """Testataan, että MemberManagerin alustus luo tarvittavat käyttöliittymäelementit"""
        # Luodaan jäljitelmillä MemberManager-instanssi
        with patch("tkinter.ttk.LabelFrame"), \
             patch("tkinter.ttk.Entry"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Button"), \
             patch("tkinter.ttk.Treeview"), \
             patch("tkinter.ttk.Scrollbar"):
            
            member_manager = MemberManager(self.mock_parent, self.db, self.mock_app)

            # Tarkistetaan, luotiinko kehys
            self.assertIsNotNone(member_manager.frame)

            # Tarkistetaan, luotiinko puunäkymä
            self.assertIsNotNone(member_manager.tree)

    def test_transaction_manager_initialization(self):
        """Testataan, että TransactionManagerin alustus luo tarvittavat käyttöliittymäelementit"""
        # Luodaan jäljitelmillä TransactionManager-instanssi
        with patch("tkinter.ttk.LabelFrame"), \
             patch("tkinter.ttk.Entry"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Button"), \
             patch("tkinter.ttk.Treeview"), \
             patch("tkinter.ttk.Scrollbar"):
            
            transaction_manager = TransactionManager(self.mock_parent, self.db, self.mock_app)

            # Tarkistetaan, luotiinko kehys
            self.assertIsNotNone(transaction_manager.frame)

            # Tarkistetaan, luotiinko puunäkymä
            self.assertIsNotNone(transaction_manager.tree)

    def test_add_book_ui(self):
        """Testataan käyttöliittymän vuorovaikutusta kirjaa lisätessä sekä virheenkäsittelyä"""
        # BookManager-instanssi
        with patch("tkinter.ttk.LabelFrame"), \
             patch("tkinter.ttk.Entry"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Button"), \
             patch("tkinter.ttk.Treeview"), \
             patch("tkinter.ttk.Scrollbar"), \
             patch("book_management.messagebox.showinfo") as mock_showinfo, \
             patch("book_management.messagebox.showerror") as mock_showerror:
                
            book_manager = BookManager(self.mock_parent, self.db, self.mock_app)

            # Jäljitellään entryjä
            book_manager.title_var = MagicMock()
            book_manager.title_var.get.return_value = "Python testaajan käsikirja"
            book_manager.author_var = MagicMock()
            book_manager.author_var.get.return_value = "Testaaja"
            book_manager.isbn_var = MagicMock()
            book_manager.isbn_var.get.return_value = "354695464534"

            # Kutsutaan funktiota, jonka pitäisi käsitellän kirjan lisäys
            book_manager.add_book()

            # Tarkistetaan, lisättiinkö kirja onnistuneesti (messageboxin avulla)
            mock_showinfo.assert_called()

            # Luodaan jäljitelmä poikkeusta varten
            self.mock_cursor.execute.side_effect = Exception("Tietokantavirhe")

            # Kutsutaan funktiota, jonka pitäisi käsitellä virhe
            book_manager.add_book()

            # Tarkistetaan, käsiteltiinkö virhe (messageboxin avulla)
            mock_showerror.assert_called()

    def test_add_member_ui(self):
        """Testataan käyttöliittymän vuorovaikutusta jäsentä lisätessä sekä virheenkäsittelyä"""
        # MemberManager-instanssi
        with patch("tkinter.ttk.LabelFrame"), \
             patch("tkinter.ttk.Entry"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Button"), \
             patch("tkinter.ttk.Treeview"), \
             patch("tkinter.ttk.Scrollbar"), \
             patch("member_management.messagebox.showinfo") as mock_showinfo, \
             patch("member_management.messagebox.showerror") as mock_showerror:
            
            member_manager = MemberManager(self.mock_parent, self.db, self.mock_app)

            # Jäljitellään entryjä
            member_manager.name_var = MagicMock()
            member_manager.name_var.get.return_value = "Testaaja"
            member_manager.email_var = MagicMock()
            member_manager.email_var.get.return_value = "testi@esimerkki.fi"
            member_manager.phone_var = MagicMock()
            member_manager.phone_var.get.return_value = "0401234567"

            # Kutsutaan funktiota, jonka pitäisi käsitellä jäsenen lisääminen
            member_manager.add_member()

            # Tarkistetaan, lisättinkö jäsen onnistuneesti (messageboxin avulla)
            mock_showinfo.assert_called()

            # Luodaan jäljitelmä poikkeusta varten
            self.mock_cursor.execute.side_effect = Exception("Tietokantavirhe")

            # Kutsutaan funktiota, jonka pitäisi käsitellä virhe
            member_manager.add_member()

            # Tarkistetaan, käsiteltiinkö virhe (messageboxin avulla)
            mock_showerror.assert_called()

    def test_borrow_and_return_book_ui(self):
        """Testataan käyttöliittymän vuorovaikutusta kirjaa lainatessa ja palauttaessa. Testataan myös virheenkäsittelyä"""
        # TransactionManager-instanssi
        with patch("tkinter.ttk.LabelFrame"), \
             patch("tkinter.ttk.Entry"), \
             patch("tkinter.ttk.Label"), \
             patch("tkinter.ttk.Button"), \
             patch("tkinter.ttk.Treeview"), \
             patch("tkinter.ttk.Scrollbar"), \
             patch("transaction_management.messagebox.showinfo") as mock_showinfo, \
             patch("transaction_management.messagebox.showerror") as mock_showerror:
            
            transaction_manager = TransactionManager(self.mock_parent, self.db, self.mock_app)

            # Jäljitellään entryjä
            transaction_manager.book_id_var = MagicMock()
            transaction_manager.book_id_var.get.return_value = "1"
            transaction_manager.member_id_var = MagicMock()
            transaction_manager.member_id_var.get.return_value = "1"

            self.mock_cursor.fetchone.return_value = {"available": 1}

            # Kutsutaan funktiota, jonka pitäisi käsitellä kirjan lainaus
            transaction_manager.borrow_book()

            # Tarkistetaan, lainattiinko kirja onnistuneesti (messageboxin avulla)
            mock_showinfo.assert_called()

            self.mock_cursor.fetchone.return_value = {"available": 0}

            # Kutsutaan funktiota, jonka pitäisi käsitellä kirjan palautus
            transaction_manager.return_book()

            # Tarkistetaan, palautettiinko kirja onnistuneesti (messageboxin avulla)
            mock_showinfo.assert_called()

            # Luodaan jäljitelmä poikkeusta varten
            self.mock_cursor.execute.side_effect = Exception("Tietokantavirhe")

            # Kutsutaan funktiota, jonka pitäisi käsitellä virhe
            transaction_manager.return_book()

            # Tarkistetaan, käsiteltiinkö virhe (messageboxin avulla)
            mock_showerror.assert_called()

if __name__ == "__main__":
    unittest.main()