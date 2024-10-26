export type Datetime = {
    year: number;
    month: number;
    day: number;
    dayOfWeek: number;
    hour: number;
    minute: number;
    second: number;
    dayName: string;
    monthName: string;
};

export class DateFormatter {
    public static toDatetime(date: Date): Datetime {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');

        return {
            year: year,
            month: Number(month),
            day: Number(day),
            dayOfWeek: date.getDay(),
            hour: date.getHours(),
            minute: date.getMinutes(),
            second: date.getSeconds(),
            dayName: date.toLocaleDateString('en-US', { weekday: 'long' }),
            monthName: date.toLocaleDateString('en-US', { month: 'long' })
        };
    }

    public static toDateString(date: Date): string {
        return `${date.toLocaleDateString('en-US', { month: 'long' })} ${date.getDate()}, ${date.getFullYear()}`;
    }
}