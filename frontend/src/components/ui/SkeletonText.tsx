import "./Spinner.css";

function getRandomIntInRange(min: number, max: number) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

export const SkeletonText = ({linesCount = 30}) => {

    const createLinesWithRandomLength = () => {
        const lines = [];
        for (let i = 0; i < linesCount; i++) {
            const randomLength = getRandomIntInRange(40, 100);
            lines.push(<div className={`skeleton-text`} style={{width: `${randomLength}%`}} key={i}></div>);
        }
        return lines;
    };
    return (
        <div className="">
            {createLinesWithRandomLength()}
        </div>
    );
};