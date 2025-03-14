from database import Database
from tkinter import ttk, messagebox
from book_management import BookManager
from member_management import MemberManager
from transaction_management import TransactionManager
import tkinter as tk

class LibraryApp:
    def __init__(self, root, db_config=None):
        self.root = root
        self.root.title("Kirjastojärjestelmä")
        self.root.geometry("800x600")
        self.root.minsize(800, 600)

        # Alustetaan tietokantayhteys
        if db_config is None:
            db_config = {"host": "localhost", "user": "root", "password": "root", "database": "LibraryDB"}
        try:
            self.db = Database(**db_config)
        except Exception as e:
            messagebox.showerror("Tietokantavirhe", f"Tietokantaan yhdistäminen epäonnistui: {str(e)}")
            root.destroy()
            return
        
        # Luodaan notebook (välilehtiä varten)
        self.notebook = ttk.Notebook(root)
        self.notebook.pack(fill="both", expand=True, padx=10, pady=10)

        # Luodaan välilehdet
        self.book_tab = ttk.Frame(self.notebook)
        self.member_tab = ttk.Frame(self.notebook)
        self.transaction_tab = ttk.Frame(self.notebook)

        self.notebook.add(self.book_tab, text="Kirjat")
        self.notebook.add(self.member_tab, text="Jäsenet")
        self.notebook.add(self.transaction_tab, text="Tapahtumat")

        self.book_management = BookManager(self.book_tab, self.db, self)
        self.member_management = MemberManager(self.member_tab, self.db, self)
        self.transaction_management = TransactionManager(self.transaction_tab, self.db, self)

        # Status-palkki
        self.status_bar = ttk.Label(root, text="Kirjastojärjestelmä - Valmis", relief=tk.SUNKEN, anchor=tk.W)
        self.status_bar.pack(side=tk.BOTTOM, fill=tk.X)

        root.protocol("WM_DELETE_WINDOW", self.on_close)

    def on_close(self):
        """Käsitellään ohjelman sulkemistapahtuma"""
        try:
            # Siivotaan tietokantayhteys
            if hasattr(self, 'db'):
                del self.db
        except:
            pass

        self.root.destroy()