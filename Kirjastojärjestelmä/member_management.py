from tkinter import ttk, messagebox

class MemberManager:
    def __init__(self, parent, db, app):
        self.parent = parent
        self.db = db
        self.app = app

        # Luodaan kehys jäsenien hallintaan
        self.frame = ttk.LabelFrame(parent, text="Jäsenien halinnta")
        self.frame.pack(fill="both", expand=True, padx=20, pady=10)

        # Luodaan entryt jäsenien kentille
        ttk.Label(self.frame, text="Nimi:").grid(row=0, column=0, padx=5, pady=5, sticky="w")
        self.name_var = ttk.Entry(self.frame, width=30)
        self.name_var.grid(row=1, column=1, padx=5, pady=5)

        ttk.Label(self.frame, text="Sähköposti:").grid(row=1, column=0, padx=5, pady=5, sticky="w")
        self.email_var = ttk.Entry(self.frame, width=30)
        self.email_var.grid(row=1, column=1, padx=5, pady=5)

        ttk.Label(self.frame, text="Puhelinnumero:").grid(row=2, column=0, padx=5, pady=5, sticky="w")
        self.phone_var = ttk.Entry(self.frame, width=30)
        self.phone_var.grid(row=2, column=1, padx=5, pady=5)

        # "Lisää jäsen" -nappi
        self.add_button = ttk.Button(self.frame, text="Lisää jäsen", command=self.add_member)
        self.add_button.grid(row=3, column=0, columnspan=2, padx=5, pady=10)

        # Hakukenttä
        ttk.Label(self.frame, text="Hae:").grid(row=4, column=0, padx=5, pady=5, sticky="w")
        self.search_var = ttk.Entry(self.frame, width=30)
        self.search_var.grid(row=4, column=1, padx=5, pady=5)

        # Hakunappi
        self.search_button = ttk.Button(self.frame, text="Hae", command=self.search_members)
        self.search_button.grid(row=5, column=0, padx=5, pady=5)

        # "Näytä kaikki" -nappi
        self.show_all_button = ttk.Button(self.frame, text="Näytä kaikki", command=self.load_members)
        self.show_all_button.grid(row=5, column=1, padx=5, pady=5)

        # Jäsenien puunäkymä
        self.tree = ttk.Treeview(self.frame, columns=("ID", "Name", "Email", "Phone"), show="headings")
        self.tree.heading("ID", text="ID")
        self.tree.heading("Name", text="Nimi")
        self.tree.heading("Email", text="Sähköposti")
        self.tree.heading("Phone", text="Puhelinnumero")

        self.tree.column("ID", width=50)
        self.tree.column("Name", width=150)
        self.tree.column("Email", width=200)
        self.tree.column("Phone", width=150)

        self.tree.grid(row=6, column=0, columnspan=2, padx=5, pady=5)

        # Vierityspalkki puunäkymälle
        scrollbar = ttk.Scrollbar(self.frame, orient="vertical", command=self.tree.yview)
        scrollbar.grid(row=6, column=2, sticky="ns")
        self.tree.configure(yscrollcommand=scrollbar.set)

        # Ladataan jäsenet aluksi
        self.load_members()

    def add_member(self):
       """Lisätään uusi jäsen tietokantaan""" 
       name = self.name_var.get()
       email = self.email_var.get()
       phone = self.phone_var.get()

       if not name or not email or not phone:
           messagebox.showerror("Virhe", "Täytä kaikki kentät")
           return
       
       try:
            self.db.add_member(name, email, phone)
            messagebox.showinfo("Onnistui", "Jäsen lisätty onnistuneesti")
            self.clear_fields()
            self.load_members()
       except Exception as e:
           messagebox.showerror("Virhe", f"Jäsenen lisääminen epäonnistui: {str(e)}")

    def load_members(self):
        """Ladataan kaikki jäsenent puunäkymään"""
        # Siivotaan edelliset alkiot 
        for item in self.tree.get_children():
            self.tree.delete(item)

        # Lisätään jäsen/jäseniä
        members = self.db_get_all_members()
        for member in members:
            self.tree.insert("", "end", values=(member['id'], member['name'], member['email'], member['phone']))
        
    def search_members(self):
        """Haetaan jäseniä hakusanan perusteella"""
        search_term = self.search_var.get()

        if not search_term:
            return
        
        # Siivotaan edelliset alkiot
        for item in self.tree.get_children():
            self.tree.delete(item)

        # Etsitään ja lisätään jäseniä
        members = self.db.search_members(search_term)
        for member in members:
            self.tree.insert("", "end", values=(member['id'], member['name'], member['email'], member['phone']))

    def clear_fields(self):
        # Tyhjennetään kentät
        self.name_var.delete(0, "end")
        self.email_var.delete(0, "end")
        self.phone_var.delete(0, "end")