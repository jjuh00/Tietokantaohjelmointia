from tkinter import ttk, messagebox

class BookManager:
    def __init__(self, parent, db, app):
        self.parent = parent
        self.db = db
        self.app = app

        # Luodaan kehys kirjojen hallintaan
        self.frame = ttk.LabelFrame(parent, text="Kirjojen hallinta")
        self.frame.pack(fill="both", expand=True, padx=20, pady=20)

        # Luodaan entryt kirjan kentille
        ttk.Label(self.frame, text="Kirjan nimi:").grid(row=0, column=0, padx=5, pady=5, sticky="w")
        self.title_var = ttk.Entry(self.frame, width=30)
        self.title_var.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(self.frame, text="Kirjailija:").grid(row=1, column=0, padx=5, pady=5, sticky="w")
        self.author_var = ttk.Entry(self.frame, width=30)
        self.author_var.grid(row=1, column=1, padx=5, pady=5)

        ttk.Label(self.frame, text="ISBN:").grid(row=2, column=0, padx=5, pady=5, sticky="w")
        self.isbn_var = ttk.Entry(self.frame, width=30)
        self.isbn_var.grid(row=2, column=1, padx=5, pady=5)

        # "Lisää kirja" -nappi
        self.add_button = ttk.Button(self.frame, text="Lisää kirja", command=self.add_book)
        self.add_button.grid(row=3, column=0, columnspan=2, padx=5, pady=10)

        # Hakukenttä
        ttk.Label(self.frame, text="Hae:").grid(row=4, column=0, padx=5, pady=5, sticky="w")
        self.search_var = ttk.Entry(self.frame, width=30)
        self.search_var.grid(row=4, column=1, padx=5, pady=5)

        # Hakunappi
        self.search_button = ttk.Button(self.frame, text="Hae", command=self.search_books)
        self.search_button.grid(row=5, column=0, padx=5, pady=5)

        # "Näytä kaikki" -nappi
        self.show_all_button = ttk.Button(self.frame, text="Näytä kaikki", command=self.load_books)
        self.show_all_button.grid(row=5, column=1, padx=5, pady=5)

        # Kirjojen puunäkymä
        self.tree = ttk.Treeview(self.frame, columns=("ID", "Title", "Author", "ISBN", "Available"), show="headings")
        self.tree.heading("ID", text="ID")
        self.tree.heading("Title", text="Kirjan nimi")
        self.tree.heading("Author", text="Kirjailija")
        self.tree.heading("ISBN", text="ISBN")
        self.tree.heading("Available", text="Saatavilla")

        self.tree.column("ID", width=50)
        self.tree.column("Title", width=200)
        self.tree.column("Author", width=150)
        self.tree.column("ISBN", width=100)
        self.tree.column("Available", width=100)

        self.tree.grid(row=6, column=0, columnspan=2, padx=5, pady=5)

        # Vierityspalkki puunäkymälle
        scrollbar = ttk.Scrollbar(self.frame, orient="vertical", command=self.tree.yview)
        scrollbar.grid(row=6, column=2, sticky="ns")
        self.tree.configure(yscrollcommand=scrollbar.set)

        # Ladataan kirjan aluksi
        self.load_books()

    def add_book(self):
        """Lisätään uusi kirja tietokantaan"""
        title = self.title_var.get()
        author = self.author_var.get()
        isbn = self.isbn_var.get()

        if not title or not author or not isbn:
            messagebox.showerror("Virhe", "Täytä kaikki kentät")
            return
        
        try:
            self.db.add_book(title, author, isbn)
            messagebox.showinfo("Onnistui", "Kirja lisätty onnistuneesti")
            self.clear_fields()
            self.load_books()
        except Exception as e:
            messagebox.showerror("Virhe", f"Kirjan lisääminen epäonnistui: {str(e)}")

    def load_books(self):
        """Ladataan kaikki kirjat puunäkymään"""
        # Siivotaan edelliset alkiot 
        for item in self.tree.get_children():
            self.tree.delete(item)

        # Lisätään kirja/kirjoja
        books = self.db.get_all_books()
        for book in books:
            available = "Yes" if book['available'] else "No"
            self.tree.insert("", "end", values=(book['id'], book['title'], book['author'], book['isbn'], available))
    
    def search_books(self):
        """Haetaan kirjoja hakusanan perusteella"""
        search_term = self.search_var.get()

        if not search_term:
            return
        
        # Siivotaan edelliset alkiot
        for item in self.tree.get_children():
            self.tree.delete(item)

        # Etsitään ja lisätään kirjoja
        books = self.db.search_books(search_term)
        for book in books:
            available = "Yes" if book['available'] else "No"
            self.tree.insert("", "end", values=(book['id'], book['title'], book['author'], book['isbn'], available))

    def clear_fields(self):
        # Tyhjennetään kentät
        self.title_var.delete(0, "end")
        self.author_var.delete(0, "end")
        self.isbn_var.delete(0, "end")