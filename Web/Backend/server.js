const express = require ('express');
const app = express();


app.use(express.json());


app.get('/', (req, res) => {
    res,json({message: 'Interfix online!' });
});

app.listen(3000, () => {
    console.log('servidor rodando na porta 3000')
})