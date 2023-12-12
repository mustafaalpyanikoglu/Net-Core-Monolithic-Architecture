namespace WebAPI.Models.Constants;

public static class SimulatedAnnealingConstants
{
    public const double INITAL_TEMPERATURE = 100;  // Başlangıç sıcaklığı
    public const double COOLINGRATE = 0.95;        // Soğuma oranı
    public const int MAX_ITERATIONS = 25000;        // Her sıcaklık için iterasyon sayısı
}
