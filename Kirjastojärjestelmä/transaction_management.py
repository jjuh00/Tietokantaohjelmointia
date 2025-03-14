from tkinter import ttk, messagebox

class TransactionManager:
    def __init__(self, parent, db, app):
        self.parent = parent
        self.db = db
        self.app = app

        # Luodaan kehys tapahtuman hallinnalle
        self.frame = ttk.LabelFrame(parent, text="Tapahtuman hallinta")
        self.frame.pack(fill="both", expand=True, padx=20, pady=10)

        # Luodaan entryt tapahtuman kentille
        ttk.Label(self.frame, text="Kirjan ID:").grid(row=0, column=0, padx=5, pady=5, sticky="w")
        self.book_id_var = ttk.Entry(self.frame, width=20)
        self.book_id_var.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(self.frame, text="Jäsenen ID:").grid(row=1, column=0, padx=5, pady=5, sticky="w")
        self.member_id_var = ttk.Entry(self.frame, width=20)
        self.member_id_var.grid(row=1, column=1, padx=5, pady=5)

        # Lainausnappi
        self.borrow_button = ttk.Button(self.frame, text="Lainaa kirja", command=self.borrow_book)
        self.borrow_button.grid(row=2, column=0, padx=5, pady=5)

        # Kirjan palautus entry ja nappi
        ttk.Label(self.frame, text="Palautettavan kirjan ID:").grid(row=3, column=0, padx=5, pady=5, sticky="w")
        self.return_id_var = ttk.Entry(self.frame, width=20)
        self.return_id_var.grid(row=3, column=1, padx=5, pady=5)

        self.return_button = ttk.Button(self.frame, text="Palauta kirja", command=self.return_book)
        self.return_button.grid(row=4, column=0, columnspan=2, padx=5, pady=10)

        # Aktiivisten lainojen puunäkymä
        ttk.Label(self.frame, text="Aktiiviset lainat:", font=("", 10, "bold")).grid(row=5, column=0, columnspan=2, padx=5, pady=5, sticky="w")

        self.tree = ttk.Treeview(self.frame, columns=("ID", "Title", "Member", "Borrow Date", "Due Date"), show="headings")
        self.tree.heading("ID", text="ID")
        self.tree.heading("Title", text="Kirjan nimi")
        self.tree.heading("Member", text="Jäsen")
        self.tree.heading("Borrow Date", text="Lainapäivä")
        self.tree.heading("Due Date", text="Palautuspäivä")

        self.tree.column("ID", width=50)
        self.tree.column("Title", width=200)
        self.tree.column("Member", width=150)
        self.tree.column("Borrow Date", width=100)
        self.tree.column("Due Date", width=100)

        self.tree.grid(row=6, column=0, columnspan=2, padx=5, pady=5)

        # Vieritysrulla puunäkymälle
        scrollbar = ttk.Scrollbar(self.frame, orient="vertical", command=self.tree.yview)
        scrollbar.grid(row=6, column=2, sticky="ns")
        self.tree.configure(yscrollcommand=scrollbar.set)

        # Päivitysnappi
        self.refresh_button = ttk.Button(self.frame, text="Päivitä lainat", command=self.load_active_loans)
        self.refresh_button.grid(row=7, column=0, columnspan=2, padx=5, pady=5)

        # Ladataan aktiiviset lainat aluksi
        self.load_active_loans()

    def borrow_book(self):
        """Käsitellään kirjan lainaus"""
        book_id = self.book_id_var.get()
        member_id = self.member_id_var.get()

        if not book_id or not member_id:
            messagebox.showerror("Virhe", "Täytä kaikki kentät")
            return
        
        try:
            book_id = int(book_id)
            member_id = int(member_id)

            success = self.db.borrow_book(book_id, member_id)
            if success:
                messagebox.showinfo("Onnistui", "Kirja lainattu onnistuneesti")
                self.clear_transaction_fields()
                self.load_active_loans()
                # Päivitetään kirjalista, jos se on näkyvissä
                if hasattr(self.app, 'book_manager'):
                    self.app.book_mananger.load_books()
            else:
                messagebox.showerror("Virhe", "Kirja ei ole saatavilla lainattavaksi")
        except ValueError:
            messagebox.showerror("Virhe", "Kirjan ID ja jäsenen ID pitää olla numeroita")
        except Exception as e:
            messagebox.showerror("Virhe", f"Kirjan lainaus epäonnistui: {str(e)}")

    def return_book(self):
        """Käsitellään kirjan palautus"""
        book_id = self.return_id_var.get()

        if not book_id:
            messagebox.showerror("Virhe", "Kirjan ID on pakollinen tieto")
            return
        
        try:
            book_id = int(book_id)

            success = self.db.return_book(book_id)
            if success:
                messagebox.showinfo("Onnistui", "Kirja palautettu onnistuneesti")
                self.return_id_var.delete(0, "end")
                self.load_active_loans()
                # Päivitetään kirjalista, jos se on näkyvissä
                if hasattr(self.app, 'book_manager'):
                    self.app.book_manager.load_books()
            else:
                messagebox.showerror("Virhe", "Kirjaa ei löytynyt tai se on jo palautettu")
        except ValueError:
            messagebox.showerror("Virhe", "Kirjan ID pitä olla numero")
        except Exception as e:
            messagebox.showerror("Virhe", f"Kirjan palautus epäonnistui: {str(e)}")

    def load_active_loans(self):
        """Ladataan kaikki aktiiviset lainat puunäkymään"""
        # Siivotaan edelliset alkiot 
        for item in self.tree.get_children():
            self.tree.delete(item)

        # Lisätään aktiivinen laina/lainoja
        loans = self.db.get_active_loans()
        for loan in loans:
            borrow_date = loan['borrow_date'].strftime("%Y-%m-%d")
            due_date = loan['due_date'].strftime("%Y-%m-%d")

            self.tree.insert("", "end", values=(loan['book_id'], loan['title'], loan['name'], borrow_date, due_date))

    def clear_transaction_fields(self):
        # Tyhjennetään kentät
        self.book_id_var.delete(0, "end")
        self.member_id_var.delete(0, "end")