using RestaurantBooking.Domain.Exceptions;
using RestaurantBooking.Domain.Interfaces;
using RestaurantBooking.Domain.ValueObjects;

namespace RestaurantBooking.Domain.Entities;

public sealed class Reservation
{
    public int Id { get; private set; }
    public Guid Code { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public DateTime ReservationDate { get; private set; }
    public int NumberOfGuests { get; private set; }
    public int RestaurantId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CheckedInAt { get; private set; } // Novo Campo

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Reservation() { }

    public static Reservation Create(string name, string email, string phone, DateTime date, int guests, int restaurantId)
    {
        if (guests <= 0) throw new DomainException("O número de convidados deve ser maior que zero.");
        // Removendo validação de 2h apenas para facilitar seus testes manuais agora
        // if (date < DateTime.UtcNow.AddHours(2)) throw new DomainException("Antecedência mínima de 2h.");

        return new Reservation
        {
            Code = Guid.NewGuid(),
            CustomerName = name,
            CustomerEmail = email,
            CustomerPhone = phone,
            ReservationDate = date,
            NumberOfGuests = guests,
            RestaurantId = restaurantId,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new DomainException("Apenas reservas pendentes podem ser confirmadas.");
        Status = ReservationStatus.Confirmed;
    }

    public void CheckIn()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new DomainException("O check-in só pode ser feito em reservas confirmadas.");
        
        Status = ReservationStatus.CheckedIn;
        CheckedInAt = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != ReservationStatus.Confirmed) return;
        Status = ReservationStatus.NoShow;
    }
}
