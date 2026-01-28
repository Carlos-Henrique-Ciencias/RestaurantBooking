import { useState } from 'react';

export default function App() {
  const [formData, setFormData] = useState({
    customerName: '',
    customerEmail: '',
    customerPhone: '',
    reservationDate: '',
    numberOfGuests: 2,
    restaurantId: 1
  });

  const [status, setStatus] = useState({ loading: false, error: '', success: '' });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setStatus({ loading: true, error: '', success: '' });

    try {
      const response = await fetch('http://localhost:5130/api/Reservations', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ...formData,
          reservationDate: new Date(formData.reservationDate).toISOString(),
          numberOfGuests: parseInt(formData.numberOfGuests)
        })
      });

      const data = await response.json();

      if (!response.ok) throw new Error(data.error || 'Erro ao processar');

      setStatus({ loading: false, error: '', success: `Mesa Reservada! Código: ${data.reservationCode || data.code}` });
    } catch (err) {
      console.error(err);
      setStatus({ loading: false, error: "Erro de conexão. Verifique se a API está rodando.", success: '' });
    }
  };

  return (
    <div className="min-h-screen bg-neutral-950 flex items-center justify-center p-4 font-sans text-neutral-200">
      <div className="w-full max-w-md bg-neutral-900 rounded-xl shadow-2xl overflow-hidden border border-neutral-800">
        
        {/* Header Restaurante */}
        <div className="bg-gradient-to-r from-emerald-800 to-teal-800 p-6 relative">
          <div className="absolute top-2 right-4 text-4xl opacity-20">🍽️</div>
          
          {/* MUDANÇA AQUI: De Codebros para Booking */}
          <h1 className="text-2xl font-bold text-white relative z-10">Restaurante Booking</h1>
          
          <p className="text-emerald-100 text-sm opacity-90 relative z-10 uppercase tracking-wide font-semibold">desafio tecnico recebai</p>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-5">
          <div>
            <label className="block text-xs font-bold mb-1 text-emerald-500 uppercase tracking-wider">Nome do Cliente</label>
            <input 
              type="text" name="customerName" required
              className="w-full bg-neutral-950 border border-neutral-700 rounded p-3 focus:border-emerald-500 outline-none transition text-sm text-white placeholder-neutral-600"
              placeholder="Ex: Carlos Henrique"
              onChange={handleChange}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold mb-1 text-emerald-500 uppercase tracking-wider">Email</label>
              <input 
                type="email" name="customerEmail" required
                className="w-full bg-neutral-950 border border-neutral-700 rounded p-3 focus:border-emerald-500 outline-none transition text-sm text-white placeholder-neutral-600"
                placeholder="cliente@email.com"
                onChange={handleChange}
              />
            </div>
            <div>
              <label className="block text-xs font-bold mb-1 text-emerald-500 uppercase tracking-wider">Whatsapp / Tel</label>
              <input 
                type="tel" name="customerPhone" required
                className="w-full bg-neutral-950 border border-neutral-700 rounded p-3 focus:border-emerald-500 outline-none transition text-sm text-white placeholder-neutral-600"
                placeholder="(79) 9..."
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-bold mb-1 text-emerald-500 uppercase tracking-wider">Data e Hora da Reserva</label>
              <input 
                type="datetime-local" name="reservationDate" required
                className="w-full bg-neutral-950 border border-neutral-700 rounded p-3 focus:border-emerald-500 outline-none transition text-sm text-neutral-400 focus:text-white"
                onChange={handleChange}
              />
            </div>
            <div>
              <label className="block text-xs font-bold mb-1 text-emerald-500 uppercase tracking-wider">Qtd. Pessoas</label>
              <input 
                type="number" name="numberOfGuests" min="1" max="20" value={formData.numberOfGuests} required
                className="w-full bg-neutral-950 border border-neutral-700 rounded p-3 focus:border-emerald-500 outline-none transition text-sm text-white"
                onChange={handleChange}
              />
            </div>
          </div>

          <button 
            disabled={status.loading}
            className="w-full bg-emerald-700 hover:bg-emerald-600 text-white font-bold py-3 rounded transition shadow-lg shadow-emerald-900/40 mt-2 active:scale-95"
          >
            {status.loading ? 'Verificando Disponibilidade...' : 'Confirmar Mesa'}
          </button>

          {status.success && <div className="p-3 bg-emerald-900/20 border border-emerald-800 text-emerald-400 text-center rounded text-xs font-mono">{status.success}</div>}
          {status.error && <div className="p-3 bg-red-900/20 border border-red-800 text-red-400 text-center rounded text-xs font-mono">{status.error}</div>}
        </form>
      </div>
    </div>
  );
}
